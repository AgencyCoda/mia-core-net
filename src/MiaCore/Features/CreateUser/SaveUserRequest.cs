using MediatR;
using MiaCore.Models.Enums;

namespace MiaCore.Features.CreateUser
{
    public class SaveUserRequest : IRequest<MiaUserDto>
    {
        public long? Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public int Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Photo { get; set; }
        public MiaUserStatus Status { get; set; }

    }
}