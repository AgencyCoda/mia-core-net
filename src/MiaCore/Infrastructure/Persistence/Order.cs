namespace MiaCore.Infrastructure.Persistence
{
    public class Order
    {
        public string Field { get; set; }
        public OrderType Type { get; set; }
    }

    public enum OrderType
    {
        Asc,
        Desc
    }
}