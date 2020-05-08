using System;
using Niazza.KafkaMessaging;

namespace OrderManager.Console.Bus
{
    [KafkaMessage("processing_requests")]
    public class SendToProcessingMessage
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string OrderNumber { get; set; }
    }

}
