using Newtonsoft.Json.Linq;

namespace OrderManager.Domain.Storage
{
    public sealed class DataWithVersion
    {
        public int Version { get; set; }
        public string Type { get; set; }
        public JObject Data { get; set; }

        public DataWithVersion()
        {
        }

        public DataWithVersion(int version, JObject data, string type)
        {
            Version = version;
            Data = data;
            Type = type;
        }
    }
}