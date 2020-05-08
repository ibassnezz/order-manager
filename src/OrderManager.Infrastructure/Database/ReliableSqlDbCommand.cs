using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace OrderManager.Infrastructure.Database
{
    public class ReliableSqlDbCommand : DbCommand
    {
        private readonly IRetryPolicy _retryPolicy;
        private readonly DbCommand _dbCommand;

        public ReliableSqlDbCommand(DbCommand command, IRetryPolicy retryPolicy)
        {
            _dbCommand = command;
            _retryPolicy = retryPolicy;
        }

        public override string CommandText
        {
            get => _dbCommand.CommandText;
            set => _dbCommand.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => _dbCommand.CommandTimeout;
            set => _dbCommand.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => _dbCommand.CommandType;
            set => _dbCommand.CommandType = value;
        }

        public override bool DesignTimeVisible
        {
            get => _dbCommand.DesignTimeVisible;
            set => _dbCommand.DesignTimeVisible = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => _dbCommand.UpdatedRowSource;
            set => _dbCommand.UpdatedRowSource = value;
        }

        protected override DbConnection DbConnection
        {
            get => _dbCommand.Connection;
            set => _dbCommand.Connection = (NpgsqlConnection) value;
        }

        protected override DbParameterCollection DbParameterCollection =>
            _dbCommand.Parameters;

        protected override DbTransaction DbTransaction
        {
            get => _dbCommand.Transaction;
            set => _dbCommand.Transaction = (NpgsqlTransaction) value;
        }

        public override void Cancel() =>
            _dbCommand.Cancel();

        public override int ExecuteNonQuery() =>
            _retryPolicy.Execute(() => _dbCommand.ExecuteNonQuery());

        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) =>
            await _retryPolicy.ExecuteAsync(() => _dbCommand.ExecuteNonQueryAsync(cancellationToken), cancellationToken);

        public override object ExecuteScalar() =>
            _retryPolicy.Execute(() => _dbCommand.ExecuteScalar());

        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken) =>
            await _retryPolicy.ExecuteAsync(() => _dbCommand.ExecuteScalarAsync(cancellationToken),cancellationToken);

        public override void Prepare() =>
            _retryPolicy.Execute(() => _dbCommand.Prepare());

        protected override DbParameter CreateDbParameter() =>
            _dbCommand.CreateParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
            _retryPolicy.Execute(() => _dbCommand.ExecuteReader(behavior));

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _dbCommand.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}