namespace OrderManager.Domain.Aggregate
{
    public interface IAggregateRoot
    {
        int PersistentVersion { get; }
    }
}