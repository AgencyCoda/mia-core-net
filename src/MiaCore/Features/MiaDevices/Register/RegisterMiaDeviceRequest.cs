using MediatR;

namespace MiaCore.Features.MiaDevices.Register
{
    public class RegisterMiaDeviceRequest : IRequest<object>
    {
        public int Type { get; set; }
        public string Token { get; set; }

    }
}