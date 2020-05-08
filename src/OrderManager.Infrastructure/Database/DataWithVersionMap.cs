using Dapper.FluentMap.Mapping;
using OrderManager.Domain.Storage;

namespace OrderManager.Infrastructure.Database
{
    public class DataWithVersionMap: EntityMap<DataWithVersion>
    {
        public DataWithVersionMap()
        {
            Map(d => d.Type).ToColumn("event_type");
            Map(d => d.Version).ToColumn("version");
            Map(d => d.Data).ToColumn("data");
        }
    }
}
