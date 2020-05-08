using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace OrderManager.Domain.Storage
{
    public class RawDataContainer
    {
        public RawSnapshotData RawSnapshot { get; }
        public Queue<DataWithVersion> RawEvents { get; }

        public RawDataContainer(RawSnapshotData rawSnapshot, Queue<DataWithVersion> rawEvents)
        {
            RawSnapshot = rawSnapshot;
            RawEvents = rawEvents;
        }

        public bool HasSnapshot() => RawSnapshot != null;
    }
}
