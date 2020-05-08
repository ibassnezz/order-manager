using System;
using System.Collections.Generic;
using System.Linq;
using OrderManager.Events.EventStack;
using OrderManager.Events.EventStack.V1;
using V2 = OrderManager.Events.EventStack.V2;

namespace OrderManager.Domain.Aggregate
{
    public partial class Order 
    {
        private readonly List<Component> _components = new List<Component>();
        private readonly List<Product> _products = new List<Product>();

        public string OrderNumber { get; }
        public string CheckNumber { get; private set; }
        public string OrderDescription { get; private set; }
        public IReadOnlyCollection<Component> Components => _components.AsReadOnly();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
        public decimal Amount { get; private set; }
        
        
        public bool HasComponentsToProcess => _components.Any(x => !x.IsHandled);


        public void SetRequisites(string checkNumber, string orderDescription)
        {
            if ((CheckNumber?.Equals(checkNumber) ?? false) && (OrderDescription?.Equals(orderDescription) ?? false))
            {
                return;
            }

            if (!string.IsNullOrEmpty(CheckNumber) && !CheckNumber.Equals(checkNumber))
            {
                throw new ArgumentException($"{nameof(checkNumber)}. Cannot have a different number with aggregate");
            }

            if (!string.IsNullOrEmpty(OrderDescription) && !OrderDescription.Equals(orderDescription))
            {
                throw new ArgumentException($"{nameof(orderDescription)}. Cannot have a different description with aggregate");
            }

            var domainEvent = new RequisitesEvent(orderDescription, checkNumber);

            Update(domainEvent);

        }

        public void AddComponent(long componentId, decimal amount, Product[] products)
        {
            if (_components.Any(x => x.Id == componentId))
            {
                throw new ArgumentException($"{nameof(componentId)}. already exists in aggregate");
            }

            var domainEvent = new ComponentAddedEvent(
                componentId,
                amount,
                products.Select(p => new ProductSourceObject(p.ItemId, p.Quantity, p.Price)).ToArray());
            Update(domainEvent);
        }

        public void SetAllProcessed()
        {
            ProcessDone(
                _components.Where(wire => !wire.IsHandled).Select(x => x.Id).ToArray(),
                Amount,
                _products.Select(p => new V2.ItemIdQuantityPair(p.ItemId, p.Quantity)).ToArray());
        }

        public void ProcessDone(long[] componentsIds, decimal processedAmount, V2.ItemIdQuantityPair[] processedValues)
        {

            if (Amount < processedAmount)
            {
                throw new ArgumentException($"{nameof(processedAmount)} is more than {nameof(Amount)}");
            }

            var unhandledWires = _components.Where(wire => !wire.IsHandled).Select(x => x.Id).ToArray();
            if (componentsIds.Any(w => !unhandledWires.Contains(w)))
            {
                throw new ArgumentException($"{nameof(componentsIds)} has unknown ids");
            }

            var processedProductIds = _products.Select(p => p.ItemId).ToArray();
            if (processedValues.Any(p => !processedProductIds.Contains(p.ItemId)))
            {
                throw new ArgumentException($"{nameof(processedValues)} has unknown ids");
            }

            var refundedEvent = new V2.ProcessedEvent(componentsIds, processedValues, processedAmount);
            Update(refundedEvent);
        }

    }
}