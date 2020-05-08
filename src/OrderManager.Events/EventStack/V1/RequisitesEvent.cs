
namespace OrderManager.Events.EventStack.V1
{
    [EventId("V1.RequisitesEvent")]
    public class RequisitesEvent: IDomainEvent
    {
        public RequisitesEvent(string orderDescription, string checkNumber)
        {
            OrderDescription = orderDescription;
            CheckNumber = checkNumber;
        }

        public RequisitesEvent()
        {
            
        }

        public string OrderDescription { get; set; }
        public string CheckNumber { get; set; }
    }
}
