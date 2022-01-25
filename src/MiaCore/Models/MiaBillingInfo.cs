namespace MiaCore.Models
{
    public class MiaBillingInfo : BaseEntity,IDeletableEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int Type { get; set; }
        public string Company { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string LegalNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
    }
}