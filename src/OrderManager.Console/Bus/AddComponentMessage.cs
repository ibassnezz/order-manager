using System;
using Niazza.KafkaMessaging;

namespace OrderManager.Console.Bus
{
    [KafkaMessage("components_requests")]
    public class AddComponentMessage
    {
        public long Id { get; set; }

        public string OrderNumber { get; set; }

        public string CheckNumber { get; set; }

        public string OrderDescription { get; set; }

        public OrderProduct[] Products { get; set; }

        public decimal Amount { get; set; }

        public bool IsDelivery { get; set; }

    }
}