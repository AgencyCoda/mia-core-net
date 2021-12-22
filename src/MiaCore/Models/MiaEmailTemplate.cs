namespace MiaCore.Models
{
    public class MiaEmailTemplate : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ContentText { get; set; }
    }
}