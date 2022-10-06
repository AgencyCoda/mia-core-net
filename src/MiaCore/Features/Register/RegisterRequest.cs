using MediatR;

namespace MiaCore.Features.Register
{
    public class RegisterRequest : IRequest<MiaUserDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
        public string IdentificationFrontUrl { get; set; }
        public bool InstitutionRegistration { get; set; }
        public string Language { get; set; }
    }
}