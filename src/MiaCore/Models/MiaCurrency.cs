namespace MiaCore.Models
{
    public class MiaCurrency : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public decimal ValUsd { get; set; }
    }
}