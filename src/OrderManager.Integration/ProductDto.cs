namespace OrderManager.Integration
{
    public class ProductDto
    {

        public ProductDto(string itemId, decimal price, int quantity)
        {
            ItemId = itemId;
            Price = price;
            Quantity = quantity;
        }

        public int Quantity { get; set; }

        public string ItemId { get; set; }

        public decimal Price { get; set; }

    }
}