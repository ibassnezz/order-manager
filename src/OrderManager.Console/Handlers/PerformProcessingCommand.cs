using MediatR;

namespace OrderManager.Console.Handlers
{
    public class PerformProcessingCommand : IRequest
    {
        public string OrderNumber { get; }

        public PerformProcessingCommand(string orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}
