using System;
using MediatR;
using MiaCore.Models;

namespace MiaCore.Features.GetDashboardStats
{
    public class GetDashboardStatsRequest : IRequest<DashboardStat>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}