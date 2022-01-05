using MediatR;

namespace MiaCore.Features.RecoveryPassword
{
    public class ChangePasswordRecoveryRequest : IRequest<bool>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
