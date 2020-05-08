namespace OrderManager.Infrastructure.Database
{
    public class OrderComponent
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public long ComponentId { get; set; }
        public bool IsProcessed { get; set; }
        public decimal Amount { get; set; }
    }
}
