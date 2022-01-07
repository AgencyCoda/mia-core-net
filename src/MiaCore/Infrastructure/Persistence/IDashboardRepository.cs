using System;
using System.Threading.Tasks;
using MiaCore.Models;

namespace MiaCore.Infrastructure.Persistence
{
    public interface IDashboardRepository
    {
        Task<DashboardStat> GetStats(DateTime startDate, DateTime endDate);
    }
}