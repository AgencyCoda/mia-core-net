using MiaCore.Models;

namespace MiaCore.Models
{
    public class RequestChange : BaseEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int NewRole { get; set; }
        public string Message { get; set; }
        public RequestChangeStatus Status { get; set; }
    }

    public enum RequestChangeStatus
    {
        Pending,
        Accepted,
        Canceled
    }
}