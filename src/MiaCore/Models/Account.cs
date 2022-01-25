namespace MiaCore.Models
{
    public class Account : BaseEntity,IDeletableEntity
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
    }
}