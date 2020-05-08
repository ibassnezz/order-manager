using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using OrderManager.Domain.Storage;

namespace OrderManager.Infrastructure.Repository
{
    public class EventDataLayer : IEventDataLayer
    {
        private readonly IDatabaseConnectionFactory _factory;
        private readonly ILogger<EventDataLayer> _logger;

        public EventDataLayer(IDatabaseConnectionFactory factory, ILogger<EventDataLayer> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task AppendRecordsAsync(string orderNumber, RawDataContainer container, CancellationToken cancellationToken)
        {
            using var connection = await _factory.CreateConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                if (container.HasSnapshot())
                {
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            EventRepositoryQueries.InsertSnapshot,
                            new { orderNumber, version = container.RawSnapshot.LastVersion, data = container.RawSnapshot.Raw },
                            transaction,
                            cancellationToken: cancellationToken));
                }

                while (container.RawEvents.TryDequeue(out var data))
                {
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            EventRepositoryQueries.Insert,
                            new { orderNumber, version = data.Version, type = data.Type, data = data.Data },
                            transaction,
                            cancellationToken: cancellationToken));
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                _logger.LogError(e, "Event saving error");
                throw;
            }
        }

        public async Task<RawDataContainer> ReadRecordsAsync(string streamName, CancellationToken cancellationToken)
        {
            using var connection = await _factory.CreateConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
            var snapshot = await connection.QueryFirstAsync<SnapshotDao>(
                new CommandDefinition(
                    EventRepositoryQueries.GetSnapshotByOrderNumber,
                    new {orderNumber = streamName},
                    transaction,
                    cancellationToken: cancellationToken));

            var events = await connection.QueryAsync<DataWithVersion>(
                new CommandDefinition(
                    EventRepositoryQueries.GetEventsByOrderNumber,
                    new { orderNumber = streamName, version = snapshot?.LastVersion ?? -1 },
                    transaction,
                    cancellationToken: cancellationToken));

            return new RawDataContainer(
                snapshot is null ? null : new RawSnapshotData(snapshot.LastVersion, snapshot.Data),
                new Queue<DataWithVersion>(events));
        }
    }
}