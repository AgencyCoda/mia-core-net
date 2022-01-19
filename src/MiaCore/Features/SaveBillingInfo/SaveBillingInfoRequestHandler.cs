using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.SaveBillingInfo
{
    internal class SaveBillingInfoRequestHandler : IRequestHandler<SaveBillingInfoRequest, MiaBillingInfo>
    {
        private readonly IGenericRepository<MiaBillingInfo> _repo;
        private readonly IMapper _mapper;
        private readonly UserHelper _userHelper;

        public SaveBillingInfoRequestHandler(IGenericRepository<MiaBillingInfo> repo, IMapper mapper, UserHelper userHelper)
        {
            _repo = repo;
            _mapper = mapper;
            _userHelper = userHelper;
        }

        public async Task<MiaBillingInfo> Handle(SaveBillingInfoRequest request, CancellationToken cancellationToken)
        {
            var billingInfo = _mapper.Map<MiaBillingInfo>(request);
            billingInfo.UserId = _userHelper.GetUserId();

            if (!request.Id.HasValue)
                billingInfo.Id = await _repo.InsertAsync(billingInfo);
            else
                await _repo.UpdateAsync(billingInfo);

            return billingInfo;
        }
    }
}