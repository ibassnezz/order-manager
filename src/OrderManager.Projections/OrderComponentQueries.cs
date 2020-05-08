namespace OrderManager.Projections
{
    internal class OrderComponentQueries
    {
        internal const string GetComponent = @"SELECT id, order_number, componentid, is_processed FROM order_components WHERE componentid=@componentId;";
        internal const string Insert = @"INSERT INTO order_components (order_number, componentid, amount, is_processed) VALUES (@orderNumber, @componentId, @amount, false);";
        internal const string SetProcessed = @"UPDATE order_components SET is_processed = true WHERE componentid = @componentId";
    }
}
