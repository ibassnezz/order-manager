using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Niazza.Vault;
using Npgsql;
using OrderManager.Infrastructure.Database;

namespace OrderManager.Migrator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Migration has heed started");

            var connectionString = GetConnectionString();
            var migrator = new MigratorRunner(connectionString);

            if (args.Contains("--dryrun"))
            {
                migrator.ListMigrations();
            }
            else
            {
                migrator.Migrate();
            }
            
            System.Console.WriteLine("Migration has been finished");

        }

        private static string GetConnectionString()
        {

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                  ?? throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT in not set");

            var config = new ConfigurationBuilder()
                .SetBasePath($"{Directory.GetCurrentDirectory()}")
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json")
                .AddJsonFile($"appsettings.{environmentName}.Migration.json")
                .Build();

            var optionsBuilder = new VaultConfigurationOptionsBuilder();
            var options =  optionsBuilder.ConfigureFromEnvironment().Build(vConfig => vConfig.ServiceName = "wire-manager");
            var provider = new VaultProviderFactory(options).CreateVaultProvider();
            
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Database = config.GetSection("Database:Name").Value,
                Port = config.GetSection("Database:Port").Get<int>(),
                Host = string.Format(config.GetSection("Database:Host").Value, Environment.GetEnvironmentVariable("O3_RELEASE_NAME")),
                Password = provider.GetValue(nameof(DbCredentials), nameof(DbCredentials.Password)).GetAwaiter().GetResult(),
                Username = provider.GetValue(nameof(DbCredentials), nameof(DbCredentials.UserId)).GetAwaiter().GetResult(),
                ApplicationName = config.GetSection("Database:ApplicationName").Value
            };

            return connectionStringBuilder.ToString();

        }
    }
}