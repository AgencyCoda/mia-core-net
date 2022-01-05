using MediatR;

namespace MiaCore.Features.RecoveryPassword
{
    public class ChangePasswordRecoveryRequest : IRequest<object>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
