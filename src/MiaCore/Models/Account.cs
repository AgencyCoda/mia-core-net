namespace MiaCore.Models
{
    public class Account : BaseEntity
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
    }
}