using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.MiaBilling.FetchCurrentBillingInfo
{
    public class FetchCurrentBillingInfoRequestHandler : IRequestHandler<FetchCurrentBillingInfoRequest, MiaBillingInfo>
    {
        private readonly IGenericRepository<MiaBillingInfo> _repo;
        private readonly UserHelper _userHelper;

        public FetchCurrentBillingInfoRequestHandler(IGenericRepository<MiaBillingInfo> repo, UserHelper userHelper)
        {
            _repo = repo;
            _userHelper = userHelper;
        }

        public async Task<MiaBillingInfo> Handle(FetchCurrentBillingInfoRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetLastByAsync(new Where(nameof(MiaBillingInfo.UserId), _userHelper.GetUserId()));
        }
    }
}