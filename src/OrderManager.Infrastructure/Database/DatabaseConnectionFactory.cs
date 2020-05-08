using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Npgsql;

namespace OrderManager.Infrastructure.Database
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;
        private readonly IRetryPolicy _retryPolicy;

        public DatabaseConnectionFactory(IOptions<DbConfiguration> configuration)
        {
            if (configuration.Value.RetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(configuration.Value.RetryCount));

            _retryPolicy = new DatabaseCommunicationRetryPolicy(
                configuration.Value.RetryCount,
                TimeSpan.FromMilliseconds(configuration.Value.CooldownIntervalMs));

            var builder = new NpgsqlConnectionStringBuilder
            {
                ApplicationName = configuration.Value.ApplicationName,
                Database = configuration.Value.Database,
                Port = configuration.Value.Port,
                Username = configuration.Value.Credentials.UserId,
                Password = configuration.Value.Credentials.Password,
                Host = configuration.Value.Host,
                Pooling = true,
                Timeout = 5,
                CommandTimeout = 8,
                TcpKeepAlive = true,
                KeepAlive = 300,
                MinPoolSize = 5,
                MaxPoolSize = 100,
                NoResetOnClose = true,
                ConnectionIdleLifetime = 300
            };

            _connectionString = builder.ToString();
        }

        public IDbConnection CreateConnection() =>
            CreateConnection(_connectionString);

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken) =>
            await CreateConnectionAsync(_connectionString, cancellationToken);

        private IDbConnection CreateConnection(string connectionString)
        {
            var connection = new ReliableSqlDbConnection(connectionString, _retryPolicy);
            connection.Open();

            return connection;
        }

        private async Task<IDbConnection> CreateConnectionAsync(string connectionString, CancellationToken cancellationToken)
        {
            var connection = new ReliableSqlDbConnection(connectionString, _retryPolicy);
            await connection.OpenAsync(cancellationToken);

            return connection;
        }
    }
}