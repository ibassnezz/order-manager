using Newtonsoft.Json.Linq;

namespace OrderManager.Domain.Storage
{
    public class SnapshotDao
    {
        public int LastVersion { get; set; }
        public JObject Data { get; set; }
    }
}
