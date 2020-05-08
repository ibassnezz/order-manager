using System;

namespace OrderManager.Domain
{
    public class Product
    {
        public Product(string itemId, int quantity, decimal price)
        {
            ItemId = itemId;
            Quantity = quantity;
            Price = price;
        }

        public string ItemId { get; }

        public int Quantity { get; private set; }

        public decimal Price { get; }

        public void AddQuantity(int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentException(nameof(quantity));
            }

            Quantity += quantity;
        }

        public void SubtractQuantity(int quantity)
        {
            if (quantity < 0 || Quantity - quantity < 0)
            {
                throw new ArgumentException(nameof(quantity));
            }

            Quantity -= quantity;
        }

    }
}
