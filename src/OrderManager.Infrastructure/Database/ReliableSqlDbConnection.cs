using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace OrderManager.Infrastructure.Database
{
    public class ReliableSqlDbConnection : DbConnection
    {
        private readonly IRetryPolicy _retryPolicy;
        private readonly DbConnection _underlyingConnection;

        private string _connectionString;

        public ReliableSqlDbConnection(string connectionString, IRetryPolicy retryPolicy)
        {
            _connectionString = connectionString;
            _retryPolicy = retryPolicy;
            _underlyingConnection = new NpgsqlConnection(connectionString);
        }

        public override string ConnectionString
        {
            get => _connectionString;

            set
            {
                _connectionString = value;
                _underlyingConnection.ConnectionString = value;
            }
        }

        public override string Database => _underlyingConnection.Database;

        public override string DataSource => _underlyingConnection.DataSource;

        public override string ServerVersion => _underlyingConnection.ServerVersion;

        public override ConnectionState State => _underlyingConnection.State;

        public override void ChangeDatabase(string databaseName) =>
            _underlyingConnection.ChangeDatabase(databaseName);

        public override void Close() =>
            _underlyingConnection.Close();

        public override void Open() =>
            _retryPolicy.Execute(() =>
            {
                if (_underlyingConnection.State != ConnectionState.Open) 
                    _underlyingConnection.Open();
            });

        public override async Task OpenAsync(CancellationToken cancellationToken) =>
            await _retryPolicy.ExecuteAsync(async () =>
            {
                if (_underlyingConnection.State != ConnectionState.Open)
                    await _underlyingConnection.OpenAsync(cancellationToken);
            }, cancellationToken);

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            _underlyingConnection.BeginTransaction(isolationLevel);

        protected override DbCommand CreateDbCommand() =>
            new ReliableSqlDbCommand(_underlyingConnection.CreateCommand(), _retryPolicy);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_underlyingConnection.State == ConnectionState.Open) 
                    _underlyingConnection.Close();

                _underlyingConnection.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}