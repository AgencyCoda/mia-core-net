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

namespace MiaCore.Infrastructure.Persistence
{
    public class GenericRepository<T> : BaseRepository, IGenericRepository<T> where T : IEntity
    {
        protected List<string> Columns;
        protected string Tablename;
        public GenericRepository(IOptions<MiaCoreOptions> options) : base(options)
        {
            Tablename = getTableName();
        }

        public GenericRepository(DbConnection connection, IOptions<MiaCoreOptions> options) : base(connection, options)
        {
            Tablename = getTableName();
        }

        public virtual async Task<T> GetAsync(object id, string[] relatedEntities = null)
        {
            var where = new Where("id", id);
            var (list, _) = await getListAsync(wheres: new List<Where> { where }, relatedEntities: relatedEntities, includeCount: false);
            return list.FirstOrDefault();
        }

        public virtual async Task<T> GetByAsync(params Where[] filters)
        {
            return await GetByAsync(null, filters);
        }

        public virtual async Task<T> GetByAsync(string[] relatedEntities = null, params Where[] filters)
        {
            var (list, _) = await getListAsync(wheres: filters.ToList(), relatedEntities: relatedEntities, limit: 1, page: 1, includeCount: false);
            return list.FirstOrDefault();
        }

        public virtual async Task<T> GetFirstByAsync(params Where[] filters)
        {
            return await GetFirstByAsync(null, filters);
        }

        public virtual async Task<T> GetFirstByAsync(string[] relatedEntities = null, params Where[] filters)
        {
            var orders = new List<Order> { new Order { Field = "id", Type = OrderType.Asc } };

            var (list, _) = await getListAsync(wheres: filters.ToList(), relatedEntities: relatedEntities, limit: 1, page: 1, orders: orders, includeCount: false);
            return list.FirstOrDefault();
        }

        public virtual async Task<T> GetLastByAsync(params Where[] filters)
        {
            return await GetLastByAsync(null, filters);
        }

        public virtual async Task<T> GetLastByAsync(string[] relatedEntities = null, params Where[] filters)
        {
            var orders = new List<Order> { new Order { Field = "id", Type = OrderType.Desc } };

            var (list, _) = await getListAsync(wheres: filters.ToList(), relatedEntities: relatedEntities, limit: 1, page: 1, orders: orders, includeCount: false);
            return list.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = "select * from " + Tablename;
            if (typeof(IDeletableEntity).IsAssignableFrom(typeof(T)))
                query += " where deleted = 0";
            return await Connection.QueryAsync<T>(query);
        }

        private async Task<(IEnumerable<T>, long?)> getListAsync(List<Where> wheres = null, List<Order> orders = null, int? limit = null, int? page = null, string[] relatedEntities = null, bool includeCount = true)
        {
            var type = typeof(T);

            var types = new Type[(relatedEntities?.Length ?? 0) + 1];
            types[0] = (type);

            int i = 0;

            var queryBuilder = new DynamicQueryBuilder(type);

            if (relatedEntities != null)
                foreach (var item in relatedEntities)
                {
                    var prop = type.GetProperty(removeUndescores(item), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    var propType = prop?.PropertyType;
                    if (propType is null)
                        throw new Exception($"Error. Field:{item} not found");

                    var propertyIsList = false;
                    if (typeof(IEnumerable).IsAssignableFrom(propType))
                    {
                        propertyIsList = true;
                        propType = propType.GenericTypeArguments[0];
                    }


                    string interm = null;
                    string joinField = null;
                    if (prop.GetCustomAttribute<RelationAttribute>() is RelationAttribute attr)
                    {
                        if (attr.IntermediateEntity != null)
                            interm = attr.IntermediateEntity.Name;

                        if (attr.JoinField != null)
                            joinField = attr.JoinField;
                    }

                    if (!propertyIsList)
                        queryBuilder.WithOne(propType.Name, joinField);
                    else
                        queryBuilder.WithMany(propType.Name, intermediateTable: interm, joinField: joinField);

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
            var existingIds = new HashSet<string>();

            long? count = null;
            if (includeCount)
                count = await Connection.ExecuteScalarAsync<long>(countQuery);

            var list = await Connection.QueryAsync(query, types, obj =>
            {
                string id = obj[0].GetType().GetProperty("Id").GetValue(obj[0]).ToString();
                object currObj;
                if (!dic.TryGetValue(id, out currObj))
                {
                    dic.Add(id, currObj = obj[0]);
                    existingIds.Clear();
                }


                if (relatedEntities != null)
                    for (int i = 0; i < relatedEntities.Length; i++)
                    {
                        if (obj[i + 1] is null)
                            continue;
                        var prop = currObj.GetType().GetProperty(removeUndescores(relatedEntities[i]), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                        {
                            var res = prop.GetValue(currObj) as IList;
                            if (res is null)
                            {
                                Type t = typeof(List<>).MakeGenericType(prop.PropertyType.GenericTypeArguments[0]);
                                res = (IList)Activator.CreateInstance(t);
                                prop.SetValue(currObj, res);
                            }
                            string unique = prop.Name + obj[i + 1].GetType().GetProperty("Id").GetValue(obj[i + 1]).ToString();
                            if (!existingIds.Contains(unique))
                            {
                                existingIds.Add(unique);
                                res.Add(obj[i + 1]);
                            }
                        }
                        else
                            prop.SetValue(currObj, obj[i + 1]);
                    }
                return (T)currObj;
            }
            // , splitOn: relatedEntities.Length > 0 ? splitColumns : null
            );
            list = list.Distinct();
            return (list, count);
        }

        public async Task<GenericListResponse<T>> GetListAsync(List<Where> wheres = null, List<Order> orders = null, int? limit = null, int? page = null, string[] relatedEntities = null)
        {
            var (list, count) = await getListAsync(wheres, orders, limit, page, relatedEntities, true);
            var result = new GenericListResponse<T>
            {
                Total = count ?? 0,
                Data = list,
                CurrentPage = page is null || page <= 0 ? 1 : page.Value,
                PerPage = limit ?? 0
            };

            return result;
        }

        public virtual async Task<int> InsertAsync(T obj)
        {
            if (obj is BaseEntity entity)
                entity.CreatedAt = DateTime.Now;

            var columns = getColumns();
            var columnsToInsert = String.Join(',', columns.Select(x => convertWithUnderscores(x)));
            var columnvalues = String.Join(',', columns.Select(x => "@" + x));

            var cmd = @$"insert into {Tablename} ({columnsToInsert}) values({columnvalues});
                        SELECT LAST_INSERT_ID()";

            var res = await Connection.QuerySingleAsync<int>(cmd, obj);
            return res;
        }

        public virtual async Task<bool> UpdateAsync(T obj)
        {
            Type t = obj.GetType();
            PropertyInfo prop = t.GetProperty("Id");
            object id = prop.GetValue(obj);

            if (obj is BaseEntity entity)
                entity.UpdatedAt = DateTime.Now;

            var columns = getColumns();
            var columnsToUpdate = String.Join(',',
                    columns.Where(x => !x.Equals("Id", StringComparison.OrdinalIgnoreCase) && !x.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                    .Select(x => convertWithUnderscores(x) + " = @" + x));

            var cmd = $"update {Tablename} set {columnsToUpdate} where id = @id";

            var updated = await Connection.ExecuteAsync(cmd, obj);

            return updated > 0;
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            string query = "";
            if (typeof(IDeletableEntity).IsAssignableFrom(typeof(T)))
                query = "update " + Tablename + " set deleted=1 where id = @id and deleted=0";
            else
                query = "delete from " + Tablename + " where id = @id";

            var deleted = await Connection.ExecuteAsync(query, new { Id = id });

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

        private string removeUndescores(string name)
        => name.Replace("_", "");

        private List<string> getColumns()
        {
            if (Columns is null)
            {
                var properties = typeof(T).GetProperties().Where(x => !x.IsDefined(typeof(RelationAttribute))).ToList();
                Columns = properties.Select(x => x.Name).ToList();
            }

            return Columns;
        }

    }
}