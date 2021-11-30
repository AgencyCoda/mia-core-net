using System.Threading.Tasks;
using Dapper;
using MiaCore.Models;
using Microsoft.Extensions.Options;

namespace MiaCore.Infrastructure.Persistence
{
    internal class UserRepository : BaseRepository<MiaUser>, IUserRepository
    {
        public UserRepository(IOptions<MiaCoreOptions> options) : base(options.Value.ConnectionString)
        {
        }

        public async Task<MiaUser> GetByEmailAsync(string email)
        {
            using var conn = GetConnection();
            var query = "select * from " + Tablename + " where email = @email";
            return await conn.QueryFirstOrDefaultAsync<MiaUser>(query, new { email });
        }

        public async Task<MiaUser> LoginAsync(string email, string password)
        {
            using var conn = GetConnection();
            var query = "select * from " + Tablename + " where email = @email and password = @password";
            return await conn.QueryFirstOrDefaultAsync<MiaUser>(query, new { email, password });
        }
    }
}