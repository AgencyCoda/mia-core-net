using MediatR;

namespace MiaCore.Features.Register
{
    public class RegisterRequest : IRequest<MiaUserDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
    }
}