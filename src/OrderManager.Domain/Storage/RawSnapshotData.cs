using Newtonsoft.Json.Linq;

namespace OrderManager.Domain.Storage
{
    public class RawSnapshotData
    {
        public RawSnapshotData(int lastVersion, JObject raw)
        {
            LastVersion = lastVersion;
            Raw = raw;
        }

        public int LastVersion { get; }
        public JObject Raw { get; }
    }
}
