using System;
using Niazza.KafkaMessaging;

namespace OrderManager.Console.Bus
{
    [KafkaMessage("process_scheduling")]
    public class ScheduleProcessingMessage
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string OrderNumber { get; set; }
        public DateTimeOffset ExecuteAt { get; set; }
    }

}
