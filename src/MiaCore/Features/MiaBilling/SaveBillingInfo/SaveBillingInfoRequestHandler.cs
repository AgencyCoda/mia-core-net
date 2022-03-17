using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.MiaBilling.SaveBillingInfo
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
            MiaBillingInfo billingInfo;

            if (!request.Id.HasValue)
            {
                var userId = _userHelper.GetUserId();
                bool exists = false;
                billingInfo = await _repo.GetByAsync(new Where(nameof(MiaBillingInfo.UserId), userId));
                if (billingInfo != null)
                {
                    exists = true;
                    request.Id = billingInfo.Id;
                }
                billingInfo = _mapper.Map<SaveBillingInfoRequest, MiaBillingInfo>(request, billingInfo);
                billingInfo.UserId = userId;
                if (!exists)
                    billingInfo.Id = await _repo.InsertAsync(billingInfo);
                else
                    await _repo.UpdateAsync(billingInfo);
            }
            else
            {
                billingInfo = await _repo.GetAsync(request.Id);
                billingInfo = _mapper.Map<SaveBillingInfoRequest, MiaBillingInfo>(request, billingInfo);
                await _repo.UpdateAsync(billingInfo);
            }

            return billingInfo;
        }
    }
}