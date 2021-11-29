using System.Threading.Tasks;
using Dapper;
using MiaCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace MiaCore.Authentication
{
    internal class UserRepository : BaseRepository<MiaUser>
    {
        public UserRepository(IOptions<MiaCoreOptions> options) : base(options.Value.ConnectionString)
        {
        }

        internal async Task<MiaUser> LoginAsync(string email, string password)
        {
            using var conn = GetConnection();
            var query = "select * from " + Tablename + " where email = @email and password = @password";
            return await conn.QueryFirstOrDefaultAsync<MiaUser>(query, new { email, password });
        }
    }
}