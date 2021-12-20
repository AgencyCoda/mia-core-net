namespace MiaCore.Models
{
    public class MiaUserCategory : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int CategoryId { get; set; }
    }
}