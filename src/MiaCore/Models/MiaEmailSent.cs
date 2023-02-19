using MiaCore.Models.Enums;

namespace MiaCore.Models
{
    public class MiaEmailSent : BaseEntity, IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public int TemplateId { get; set; }
        public string Vars { get; set; }
        public string Data { get; set; }
        public MiaEmailSentStatus Status { get; set; }
    }
}