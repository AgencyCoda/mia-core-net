using MediatR;
using MiaCore.Features.Login;

namespace MiaCore.Features.Firebase
{
    public class FirebaseAuthenticationRequest : IRequest<LoginResponseDto>
    {
        public string Token { get; set; }
    }
}