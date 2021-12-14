using MediatR;

namespace MiaCore.Features.RecoveryPassword
{
    public class RecoveryPasswordRequest : IRequest<bool>
    {
        public string Email { get; set; }
    }
}