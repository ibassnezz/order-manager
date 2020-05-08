using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using OrderManager.Infrastructure;
using OrderManager.Infrastructure.Database;
using OrderManager.Infrastructure.Repository;

namespace OrderManager.Projections
{
    public class OrderComponentRepository : IOrderComponentRepository
    {
        private readonly IDatabaseConnectionFactory _factory;
        private readonly ILogger<OrderComponentRepository> _logger;

        public OrderComponentRepository(IDatabaseConnectionFactory factory, ILogger<OrderComponentRepository> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task CreateAsync(string orderNumber, long componentId, decimal amount, CancellationToken cancellationToken)
        {
            using var connection = await _factory.CreateConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        OrderComponentQueries.Insert,
                        new { orderNumber, componentId, amount },
                        cancellationToken: cancellationToken));
            
        }

        public async Task SetProcessedAsync(long componentId, CancellationToken cancellationToken)
        {
            using var connection = await _factory.CreateConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(
                new CommandDefinition(
                    OrderComponentQueries.SetProcessed,
                    new { componentId },
                    cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<OrderComponent>> GetAsync(long componentId, CancellationToken cancellationToken)
        {
            using var connection = await _factory.CreateConnectionAsync(cancellationToken);
            var components = await connection.QueryAsync<OrderComponent>(
                new CommandDefinition(
                    OrderComponentQueries.GetComponent,
                    new { componentId },
                    cancellationToken: cancellationToken));
            return components;
        }

    }
}
