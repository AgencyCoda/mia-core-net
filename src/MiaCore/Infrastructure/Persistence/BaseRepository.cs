using System;
using System.Data.Common;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace MiaCore.Infrastructure.Persistence
{
    public abstract class BaseRepository : IDisposable
    {
        protected readonly string _connectionString;
        private DbConnection _connection;
        public DbConnection Connection =>
                _connection is not null ?
                _connection
                : _connection = new MySqlConnection(_connectionString);

        public BaseRepository(IOptions<MiaCoreOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public BaseRepository(DbConnection connection, IOptions<MiaCoreOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
            _connection = connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}