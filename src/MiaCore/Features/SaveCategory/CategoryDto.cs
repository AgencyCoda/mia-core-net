namespace MiaCore.Features.SaveCategory
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }
        public string Icon { get; set; }
        public int Type { get; set; }
        public bool IsFeatured { get; set; }

    }
}