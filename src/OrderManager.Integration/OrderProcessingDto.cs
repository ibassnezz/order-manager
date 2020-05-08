namespace OrderManager.Integration
{
    public class OrderProcessingDto
    {
        public string OrderNumber { get; set; }

        public string CheckNumber { get; set; }

        public string Description { get; set; }

        public long[] ComponentIds { get; set; }

        public decimal Amount { get; set; }

        public ProductDto[] Products { get; set; }

    }
}