using MediatR;

namespace MiaCore.Features.RecoveryPassword
{
    public class RecoveryPasswordRequest : IRequest<object>
    {
        public string Email { get; set; }
    }
}