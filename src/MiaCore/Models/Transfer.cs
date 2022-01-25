namespace MiaCore.Models
{
    public class Transfer : BaseEntity, IDeletableEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public string Data { get; set; }
        public int Status { get; set; }
    }
}