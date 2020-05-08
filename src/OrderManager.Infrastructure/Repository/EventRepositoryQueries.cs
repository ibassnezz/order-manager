namespace OrderManager.Infrastructure.Repository
{
    internal static class EventRepositoryQueries
    {
        internal const string GetEventsByOrderNumber = @"SELECT event_type, version, data FROM order_events WHERE order_number=@orderNumber and version > @version order by version;";
        internal const string GetSnapshotByOrderNumber = @"SELECT last_version, data FROM order_snapshots WHERE order_number=@orderNumber order by last_version desc limit 1;";
        internal const string Insert = @"INSERT INTO order_events (order_number, version, event_type, data) VALUES (@orderNumber, @version, @type, @data);";
        internal const string InsertSnapshot = @"INSERT INTO order_snapshots (order_number, last_version, data) VALUES (@orderNumber, @version, @data);";
    }
}