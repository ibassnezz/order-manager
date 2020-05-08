namespace OrderManager.Events.Snapshot
{
    public class Snapshot<TSnapshotData> where TSnapshotData : ISnapshotData
    {
        public int LastVersion { get; }
        public TSnapshotData Payload { get; }
        public Snapshot(int lastVersion, TSnapshotData payload)
        {
            LastVersion = lastVersion;
            Payload = payload;
        }
    }
}