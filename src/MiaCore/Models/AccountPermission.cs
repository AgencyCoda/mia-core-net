namespace MiaCore.Models
{
    public class AccountPermission : IEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long UserId { get; set; }
    }
}