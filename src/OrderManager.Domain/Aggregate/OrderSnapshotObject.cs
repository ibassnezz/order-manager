using System.Collections.Generic;
using Newtonsoft.Json;
using OrderManager.Events.Snapshot;

namespace OrderManager.Domain.Aggregate
{
    public class OrderSnapshotObject : ISnapshotData
    {
        public string CheckNumber { get; set; }
        public string OrderDescription { get; set; }
        public Component[] Components { get; set; }
        public Product[] Products { get; set; }
        public decimal Amount { get; set; }

        [JsonConstructor]
        public OrderSnapshotObject(string checkNumber, string orderDescription, Component[] components, Product[] products, decimal amount)
        {
            CheckNumber = checkNumber;
            OrderDescription = orderDescription;
            Components = components;
            Products = products;
            Amount = amount;
        }
    }
}
