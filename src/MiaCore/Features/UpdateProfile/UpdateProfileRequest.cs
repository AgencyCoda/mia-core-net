using MediatR;

namespace MiaCore.Features.UpdateProfile
{
    internal class UpdateProfileRequest : IRequest<MiaUserDto>
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string Caption { get; set; }
    }
}