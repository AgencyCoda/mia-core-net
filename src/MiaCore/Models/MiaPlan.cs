namespace MiaCore.Models
{
    public class MiaPlan : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Caption { get; set; }
        public decimal PriceMonth { get; set; }
        public decimal PriceYear { get; set; }
        public string PaypalPlanId { get; set; }
        public string Data { get; set; }
        public string PaypalPlanIdYear { get; set; }
        public bool IsShow { get; set; }
    }
}