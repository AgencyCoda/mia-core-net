using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.MiaDevices.Register
{
    public class RegisterMiaDeviceRequestHandler : IRequestHandler<RegisterMiaDeviceRequest, object>
    {
        private readonly IGenericRepository<MiaDevice> _repo;
        private readonly IMapper _mapper;
        private readonly UserHelper _userHelper;
        public RegisterMiaDeviceRequestHandler(IGenericRepository<MiaDevice> repo, IMapper mapper, UserHelper userHelper)
        {
            _repo = repo;
            _mapper = mapper;
            _userHelper = userHelper;
        }

        public async Task<object> Handle(RegisterMiaDeviceRequest request, CancellationToken cancellationToken)
        {
            long userId = _userHelper.GetUserId();
            var device = _mapper.Map<MiaDevice>(request);
            device.UserId = userId;

            var existingDevice = await _repo.GetFirstByAsync(
                    new Where(nameof(MiaDevice.DeviceToken), request.Token),
                    new Where(nameof(MiaDevice.UserId), userId));
            if (existingDevice != null)
                throw new BadRequestException(ErrorMessages.DeviceAlreadyExists);

            await _repo.InsertAsync(device);
            return new { Success = true };
        }
    }
}