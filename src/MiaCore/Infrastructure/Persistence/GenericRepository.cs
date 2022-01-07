using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using MiaCore.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace MiaCore.Infrastructure.Persistence
{
    internal class GenericRepository<T> : BaseRepository, IGenericRepository<T> where T : IEntity
    {
        protected List<string> Columns;
        protected string Tablename;
        public GenericRepository(IOptions<MiaCoreOptions> options) : base(options)
        {
            Tablename = getTableName();
        }

        public virtual async Task<T> GetAsync(object id)
        {
            using var conn = GetConnection();
            var query = "select * from " + Tablename + " where id = @id";
            var result = await conn.QueryFirstOrDefaultAsync<T>(query, new { id });
            return result;
        }

        public virtual async Task<T> GetByAsync(params Where[] filters)
        {
            using var conn = GetConnection();
            var queryBuilder = new DynamicQueryBuilder(typeof(T).Name);
            var query = queryBuilder
                .Where(filters.ToList())
                .WithLimit(1, 1)
                .Build();

            return await conn.QueryFirstOrDefaultAsync<T>(query);
        }

        public virtual async Task<T> GetFirstByAsync(params Where[] filters)
        {
            using var conn = GetConnection();
            var queryBuilder = new DynamicQueryBuilder(typeof(T).Name);
            var query = queryBuilder
                .Where(filters.ToList())
                .OrderBy(new List<Order> { new Order { Field = "id", Type = OrderType.Asc } })
                .WithLimit(1, 1)
                .Build();

            return await conn.QueryFirstOrDefaultAsync<T>(query);
        }

        public virtual async Task<T> GetLastByAsync(params Where[] filters)
        {
            using var conn = GetConnection();
            var queryBuilder = new DynamicQueryBuilder(typeof(T).Name);
            var query = queryBuilder
                .Where(filters.ToList())
                .OrderBy(new List<Order> { new Order { Field = "id", Type = OrderType.Desc } })
                .WithLimit(1, 1)
                .Build();

            return await conn.QueryFirstOrDefaultAsync<T>(query);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using var conn = GetConnection();
            var query = "select * from " + Tablename;
            return await conn.QueryAsync<T>(query);
        }

        public async Task<GenericListResponse<T>> GetListAsync(string[] relatedEntities, int? limit, int? page, List<Where> wheres, List<Order> orders)
        {
            using var conn = GetConnection();

            var type = typeof(T);

            var types = new Type[(relatedEntities?.Length ?? 0) + 1];
            types[0] = (type);

            int i = 0;

            var queryBuilder = new DynamicQueryBuilder(type.Name);

            if (relatedEntities != null)
                foreach (var item in relatedEntities)
                {
                    var propType = type.GetProperty(item)?.PropertyType;
                    if (propType is null)
                        throw new Exception($"Error.Field: {item} not found");

                    var propertyIsList = false;
                    if (typeof(IEnumerable).IsAssignableFrom(propType))
                    {
                        propertyIsList = true;
                        propType = propType.GenericTypeArguments[0];
                    }

                    if (!propertyIsList)
                        queryBuilder.WithOne(propType.Name);
                    else
                        queryBuilder.WithMany(propType.Name);

                    types[i + 1] = propType;
                    i += 1;
                }

            queryBuilder = queryBuilder
                    .Where(wheres)
                    .OrderBy(orders)
                    .WithLimit(limit, page);

            var query = queryBuilder
                .Build();

            var countQuery = queryBuilder
                .WithCount()
                .Build();


            var dic = new Dictionary<string, object>();
            var count = await conn.ExecuteScalarAsync<long>(countQuery);

            var list = await conn.QueryAsync(query, types, obj =>
            {
                string id = obj[0].GetType().GetProperty("Id").GetValue(obj[0]).ToString();
                object currObj;
                if (dic.TryGetValue(id, out currObj))
                    obj[0] = currObj;
                else
                    dic.Add(id, obj[0]);

                if (relatedEntities != null)
                    for (int i = 0; i < relatedEntities.Length; i++)
                    {
                        var prop = obj[0].GetType().GetProperty(relatedEntities[0]);
                        if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                        {
                            var res = prop.GetValue(obj[0]) as IList;
                            if (res is null)
                            {
                                Type t = typeof(List<>).MakeGenericType(prop.PropertyType.GenericTypeArguments[0]);
                                res = (IList)Activator.CreateInstance(t);
                                prop.SetValue(obj[0], res);
                            }
                            res.Add(obj[i + 1]);
                        }
                        else
                            prop.SetValue(obj[0], obj[i + 1]);
                    }
                return (T)obj[0];
            }
            // , splitOn: relatedEntities.Length > 0 ? splitColumns : null
            );
            var result = new GenericListResponse<T>
            {
                Total = count,
                Data = list,
                CurrentPage = page is null || page <= 0 ? 1 : page.Value,
                PerPage = limit ?? 0
            };

            return result;
        }

        public virtual async Task<int> InsertAsync(T obj)
        {
            using var conn = GetConnection();

            if (obj is BaseEntity entity)
                entity.CreatedAt = DateTime.Now;

            var columns = getColumns();
            var columnsToInsert = String.Join(',', columns.Select(x => convertWithUnderscores(x)));
            var columnvalues = String.Join(',', columns.Select(x => "@" + x));

            var cmd = @$"insert into {Tablename} ({columnsToInsert}) values({columnvalues});
                        SELECT LAST_INSERT_ID()";

            var res = await conn.QuerySingleAsync<int>(cmd, obj);
            return res;
        }

        public virtual async Task<bool> UpdateAsync(T obj)
        {
            Type t = obj.GetType();
            PropertyInfo prop = t.GetProperty("Id");
            object id = prop.GetValue(obj);

            if (obj is BaseEntity entity)
                entity.UpdatedAt = DateTime.Now;

            using var conn = GetConnection();

            var columns = getColumns();
            var columnsToUpdate = String.Join(',',
                    columns.Where(x => !x.Equals("Id", StringComparison.OrdinalIgnoreCase) && !x.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                    .Select(x => convertWithUnderscores(x) + " = @" + x));

            var cmd = $"update {Tablename} set {columnsToUpdate} where id = @id";

            var updated = await conn.ExecuteAsync(cmd, obj);

            return updated > 0;
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            using var conn = GetConnection();
            var query = "delete from " + Tablename + " where id = @id";
            var deleted = await conn.ExecuteAsync(query, id);
            return deleted > 0;
        }

        private string getTableName()
        {
            var type = typeof(T);
            var name = convertWithUnderscores(type.Name);
            return name.ToLower();
        }

        private string convertWithUnderscores(string name)
        => string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));

        private List<string> getColumns()
        {
            if (Columns is null)
            {
                var properties = typeof(T).GetProperties().ToList();
                Columns = properties.Select(x => x.Name).ToList();
            }

            return Columns;
        }

    }
}