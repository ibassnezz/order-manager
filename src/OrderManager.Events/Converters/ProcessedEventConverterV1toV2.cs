using System.Linq;
using V1 = OrderManager.Events.EventStack.V1;
using V2 = OrderManager.Events.EventStack.V2;

namespace OrderManager.Events.Converters
{
    public class ProcessedEventConverterV1toV2 : AbstractEventConverter<V1.ProcessedEvent, V2.ProcessedEvent>
    {
        public override V2.ProcessedEvent Convert(V1.ProcessedEvent eventFrom) => new V2.ProcessedEvent(
            eventFrom.ComponentIds,
            eventFrom.ProductItemIds.Select(x => new V2.ItemIdQuantityPair(x, 0)).ToArray(),
            eventFrom.Amount);
    }
}
