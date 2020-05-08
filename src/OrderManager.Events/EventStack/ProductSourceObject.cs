namespace OrderManager.Events.EventStack
{
    public class ProductSourceObject
    {

        public ProductSourceObject()
        {
            
        }

        public ProductSourceObject(string itemId, int quantity, decimal price)
        {
            ItemId = itemId;
            Quantity = quantity;
            Price = price;
        }

        public string ItemId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
