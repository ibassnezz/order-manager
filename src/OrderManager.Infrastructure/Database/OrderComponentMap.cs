using Dapper.FluentMap.Mapping;

namespace OrderManager.Infrastructure.Database
{
    public class OrderComponentMap : EntityMap<OrderComponent>
    {
        public OrderComponentMap()
        {
            Map(d => d.ComponentId).ToColumn("componentid");
            Map(d => d.OrderNumber).ToColumn("order_number");
            Map(d => d.Id).ToColumn("id");
            Map(d => d.Amount).ToColumn("amount");
            Map(d => d.IsProcessed).ToColumn("is_processed");
        }
    }
}
