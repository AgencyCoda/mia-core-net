using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace MiaCore.Infrastructure.Persistence
{
    public abstract class BaseRepository<T> : IGenericRepository<T>
    {
        protected readonly string _connectionString;
        protected readonly DbConnection Connection;
        protected List<string> Columns;
        protected string Tablename;
        public BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
            Tablename = getTableName();
        }

        protected DbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public virtual async Task<T> GetAsync(object id)
        {
            using var conn = GetConnection();
            var query = "select * from " + Tablename + " where id = @id";
            var result = await conn.QueryFirstOrDefaultAsync<T>(query, new { id });
            return result;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using var conn = GetConnection();
            var query = "select * from " + Tablename;
            return await conn.QueryAsync<T>(query);
        }

        public virtual async Task<int> InsertAsync(T obj)
        {
            using var conn = GetConnection();

            var columns = getColumns();
            var columnsToInsert = String.Join(',', columns.Select(x => convertWithUnderscores(x)));
            var columnvalues = String.Join(',', columns.Select(x => "@" + x));

            var cmd = $"insert into {Tablename} ({columnsToInsert}) values({columnvalues})";

            var res = await conn.ExecuteAsync(cmd, obj);
            return res;
        }

        public virtual async Task<bool> UpdateAsync(T obj)
        {
            Type t = obj.GetType();
            PropertyInfo prop = t.GetProperty("Id");
            object id = prop.GetValue(obj);

            using var conn = GetConnection();

            var columns = getColumns();
            var columnsToInsert = String.Join(',',
                    columns.Where(x => !x.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    .Select(x => convertWithUnderscores(x)));

            var columnvalues = String.Join(',',
                    columns.Where(x => !x.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => "@" + x));

            var cmd = $"insert into {Tablename} ({columnsToInsert}) values({columnvalues}) where id = @id";

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