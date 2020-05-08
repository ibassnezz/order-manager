using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace OrderManager.Migrator
{
    public class MigratorRunner
    {
        private readonly string _connectionString;

        public MigratorRunner(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Migrate()
        {
            var serviceProvider = CreateServices();

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        public void ListMigrations()
        {
            var serviceProvider = CreateServices();

            using (var scope = serviceProvider.CreateScope())
            {
                var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                if (runner.HasMigrationsToApplyUp())
                {
                    runner.ListMigrations();
                }
            }
        }


        private IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                    rb.AddPostgres()
                        .WithGlobalConnectionString(_connectionString)
                        .WithVersionTable(new VersionTable())
                        .ScanIn(typeof(MigratorRunner).Assembly)
                        .For.Migrations()
                )
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                connection.ReloadTypes();
            }
        }

        


    }
}