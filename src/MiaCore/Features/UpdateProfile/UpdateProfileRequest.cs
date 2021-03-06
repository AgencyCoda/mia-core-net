using MediatR;

namespace MiaCore.Features.UpdateProfile
{
    public class UpdateProfileRequest : IRequest<MiaUserDto>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string Caption { get; set; }
        public string Language { get; set; }
        public bool OtpEnabled { get; set; }
    }
}