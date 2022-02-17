using MediatR;

namespace MiaCore.Features.Login
{
    public class LoginRequest : IRequest<LoginResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}