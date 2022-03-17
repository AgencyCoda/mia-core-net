namespace MiaCore.Models
{
    public class MiaDevice : BaseEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long DeviceType { get; set; }
        public string DeviceToken { get; set; }
    }
}