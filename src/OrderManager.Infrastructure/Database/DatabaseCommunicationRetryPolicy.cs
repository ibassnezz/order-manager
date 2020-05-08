using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Polly;

namespace OrderManager.Infrastructure.Database
{
    public class DatabaseCommunicationRetryPolicy : IRetryPolicy
    {
        
        private readonly Policy _retryPolicy;
        private readonly Policy _retryPolicyAsync;
        
        private readonly string[] _sqlExceptions =
        {
            PostgreSqlExceptionCodes.ConnectionException,
            PostgreSqlExceptionCodes.ConnectionDoesNotExist,
            PostgreSqlExceptionCodes.ConnectionFailure,
            PostgreSqlExceptionCodes.SqlClientUnableToEstablishSqlConnection,
            PostgreSqlExceptionCodes.SqlServerRejectedEstablishmentOfSqlConnection,
            PostgreSqlExceptionCodes.TransactionResolutionUnknown,
            PostgreSqlExceptionCodes.ProtocolViolation,
            PostgreSqlExceptionCodes.TooManyConnections,
            PostgreSqlExceptionCodes.ConfigurationLimitExceeded
        };

        public DatabaseCommunicationRetryPolicy(int retryCount, TimeSpan waitBetweenRetries)
        {
            _retryPolicyAsync = Policy
                .Handle<PostgresException>(exception => _sqlExceptions.Contains(exception.SqlState))
                .WaitAndRetryAsync(retryCount, attempt => waitBetweenRetries);

            _retryPolicy = Policy
                .Handle<PostgresException>(exception => _sqlExceptions.Contains(exception.SqlState))
                .WaitAndRetry(retryCount, attempt => waitBetweenRetries);
        }

        public void Execute(Action operation) =>
            _retryPolicy.Execute(operation.Invoke);

        public TResult Execute<TResult>(Func<TResult> operation) =>
            _retryPolicy.Execute(operation.Invoke);

        public async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken) =>
            await _retryPolicyAsync.ExecuteAsync(operation.Invoke);

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken) =>
            await _retryPolicyAsync.ExecuteAsync(operation.Invoke);
        
        private static class PostgreSqlExceptionCodes
        {
            public const string ConnectionException = "08000";
            public const string ConnectionDoesNotExist = "08003";
            public const string ConnectionFailure = "08006";
            public const string SqlClientUnableToEstablishSqlConnection = "08001";
            public const string SqlServerRejectedEstablishmentOfSqlConnection = "08004";
            public const string TransactionResolutionUnknown = "08007";
            public const string ProtocolViolation = "08P01";
            public const string TooManyConnections = "53300";
            public const string ConfigurationLimitExceeded = "53400";
        }
    }
}