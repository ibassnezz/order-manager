using Newtonsoft.Json;

namespace OrderManager.Events.EventStack.V2
{
    public class ItemIdQuantityPair
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }

        public ItemIdQuantityPair(string itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }

        public ItemIdQuantityPair() { }

        [JsonIgnore]
        public bool ShouldProcessAll => Quantity == 0;

    }
}
