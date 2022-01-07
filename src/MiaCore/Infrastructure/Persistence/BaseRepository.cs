using System.Data.Common;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace MiaCore.Infrastructure.Persistence
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;

        public BaseRepository(IOptions<MiaCoreOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        protected DbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

    }
}