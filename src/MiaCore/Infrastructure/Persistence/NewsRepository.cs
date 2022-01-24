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
            var where = "";
            if (categories != null && categories.Any())
            {
                where = $"where nc.category_id in ({string.Join(",", categories)})";
            }
            var query = $@"SELECT
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
                        FROM news n 
                        left join news_category nc on n.id = nc.news_id
                        {where}
                        HAVING distance < 10
                        ORDER BY distance";
            var res = await conn.QueryAsync<News>(query, new { latitude, longitude });
            return res.ToList();
        }
    }
}