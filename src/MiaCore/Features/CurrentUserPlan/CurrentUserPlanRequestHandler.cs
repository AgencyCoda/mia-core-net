using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.CurrentUserPlan
{
    internal class CurrentUserPlanRequestHandler : IRequestHandler<CurrentUserPlanRequest, MiaUserPlan>
    {
        private readonly IGenericRepository<MiaUserPlan> _planRepository;


        public CurrentUserPlanRequestHandler(IGenericRepository<MiaUserPlan> planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<MiaUserPlan> Handle(CurrentUserPlanRequest request, CancellationToken cancellationToken)
        {
            var userPlan = await _planRepository.GetLastByAsync(
                    new Where(nameof(MiaUserPlan.UserId), request.Id),
                    new Where(nameof(MiaUserPlan.Status), 1)
                    );

            return userPlan;
        }
    }
}