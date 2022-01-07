using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.GetDashboardStats
{
    public class GetDashboardStatsRequestHandler : IRequestHandler<GetDashboardStatsRequest, DashboardStat>
    {
        private readonly IDashboardRepository _repo;
        public GetDashboardStatsRequestHandler(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<DashboardStat> Handle(GetDashboardStatsRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetStats(request.StartDate, request.EndDate);
        }
    }
}