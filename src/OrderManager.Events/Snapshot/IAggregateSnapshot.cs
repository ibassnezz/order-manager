namespace OrderManager.Events.Snapshot
{
    public interface IAggregateSnapshot<TSnapshotData> where TSnapshotData : ISnapshotData
    {
        void Restore(TSnapshotData snapshot);
        TSnapshotData Catch();
    }
}
