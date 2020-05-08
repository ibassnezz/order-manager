using MediatR;

namespace OrderManager.Console.Handlers
{
    public class CreateOrderComponentCommand : IRequest
    {
        public CreateOrderComponentCommand(
            long componentId, 
            string orderNumber, 
            string billNumber, 
            string paymentsType, 
            ComponentProduct[] products, 
            decimal amount, 
            bool isDelivery)
        {
            ComponentId = componentId;
            OrderNumber = orderNumber;
            BillNumber = billNumber;
            PaymentsType = paymentsType;
            Products = products;
            Amount = amount;
        }

        public long ComponentId { get; }
        public string OrderNumber { get; }
        public string BillNumber { get; }
        public string PaymentsType { get; }
        public ComponentProduct[] Products { get; }
        public decimal Amount { get; }
    }

    public class ComponentProduct
    {
        public ComponentProduct(string itemId, int quantity, decimal price)
        {
            ItemId = itemId;
            Quantity = quantity;
            Price = price;
        }

        public string ItemId { get; }
        public int Quantity { get; }
        public decimal Price { get; }
    }
}