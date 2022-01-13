using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MiaCore.Models;
using Microsoft.Extensions.Options;

namespace MiaCore.Infrastructure.Persistence
{
    internal class NewsRepository : GenericRepository<News>, INewsRepository
    {
        public NewsRepository(IOptions<MiaCoreOptions> options) : base(options)
        {
        }

        public async Task<List<News>> SearchByLocationAsync(double latitude, double longitude, List<int> categories)
        {
            using var conn = GetConnection();
            var query = @"SELECT
                            *,
                            ( 6371
                            * acos( cos( radians(@latitude) )
                                    * cos(  radians( latitude )   )
                                    * cos(  radians( longitude ) - radians(longitude) )
                                    + sin( radians(@latitude) )
                                    * sin( radians( latitude ) )
                                    )
                            )
                            AS distance
                        FROM news
                        HAVING distance < 10
                        ORDER BY distance";
            var res = await conn.QueryAsync<News>(query, new { latitude, longitude, categories = string.Join(",", categories) });
            return res.ToList();
        }
    }
}