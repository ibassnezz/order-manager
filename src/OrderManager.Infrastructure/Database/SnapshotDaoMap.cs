using Dapper.FluentMap.Mapping;
using OrderManager.Domain.Storage;

namespace OrderManager.Infrastructure.Database
{
    public class SnapshotDaoMap : EntityMap<SnapshotDao>
    {
        public SnapshotDaoMap()
        {
            Map(x => x.Data).ToColumn("data");
            Map(x => x.LastVersion).ToColumn("last_version");
        }
    }
}
