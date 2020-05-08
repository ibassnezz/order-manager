using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Niazza.KafkaMessaging.Producer;
using OrderManager.Console.Bus;

namespace OrderManager.Console.Application
{
    public class ScheduleService
    {
        private readonly IAsyncProducer _asyncProducer;
        private readonly TimeSpan _processSchedulePeriod;

        public ScheduleService(IOptions<ScheduleConfiguration> options, IAsyncProducer asyncProducer)
        {
            _asyncProducer = asyncProducer;
            _processSchedulePeriod = options.Value.SchedulePeriod;
        }

        public Task Schedule(string orderNumber, CancellationToken cancellationToken)
        {
            return _asyncProducer.ProduceAsync(
                new ScheduleProcessingMessage
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = orderNumber,
                    CreatedAt = DateTimeOffset.UtcNow,
                    ExecuteAt = DateTimeOffset.UtcNow + _processSchedulePeriod
                },
                cancellationToken);
        }
    }
}
