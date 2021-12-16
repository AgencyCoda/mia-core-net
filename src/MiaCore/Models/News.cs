namespace MiaCore.Models
{
    public class News : BaseEntity, IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Likes { get; set; }
        public long ParentId { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AddressData { get; set; }
        public int Status { get; set; }
        public bool Visibility { get; set; }
        public bool IsLive { get; set; }
        public bool Deleted { get; set; }
    }
}