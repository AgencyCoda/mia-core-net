using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.MiaBilling.FetchMiaBillingInfo
{
    internal class FetchMiaBillingInfoRequestHandler : IRequestHandler<FetchMiaBillingInfoRequest, MiaBillingInfo>
    {
        private readonly IGenericRepository<MiaBillingInfo> _repo;

        public FetchMiaBillingInfoRequestHandler(IGenericRepository<MiaBillingInfo> repo)
        {
            _repo = repo;
        }

        public async Task<MiaBillingInfo> Handle(FetchMiaBillingInfoRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetLastByAsync(new Where(nameof(MiaBillingInfo.UserId), request.Id));
        }
    }
}