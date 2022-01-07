using System;
using System.Threading.Tasks;
using Dapper;
using MiaCore.Models;
using Microsoft.Extensions.Options;

namespace MiaCore.Infrastructure.Persistence
{
    public class DashboardRepository : BaseRepository, IDashboardRepository
    {
        public DashboardRepository(IOptions<MiaCoreOptions> options) : base(options)
        {
        }

        public async Task<DashboardStat> GetStats(DateTime startDate, DateTime endDate)
        {
            using var connection = GetConnection();
            var res = await connection.QueryFirstOrDefaultAsync<DashboardStat>(
                @"
                select 
                (select round(Sum(amount),2) from ewire.transfer t where t.created_at between @startDate and @endDate) as TotalTokens,
                (select count(1) from ewire.account t where t.created_at between @startDate and @endDate) as TotalAccounts,
                (select count(1) from ewire.news t where t.created_at between @startDate and @endDate) as TotalNews,
                (select round(Sum(amount),2) from ewire.transfer t where t.created_at between @startDate and @endDate and type=2) as TotalRewards,
                (select count(1) from ewire.transfer t where t.created_at between @startDate and @endDate) as TotalTransfers,
                (select count(1) from ewire.mia_user_plan t where t.created_at between '@startDate' and '@endDate') as TotalPlans
                ",
                 new { startDate, endDate }
            );

            return res;
        }
    }
}