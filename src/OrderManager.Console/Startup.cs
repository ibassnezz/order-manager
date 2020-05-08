using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Niazza.KafkaMessaging;
using Niazza.KafkaMessaging.Consumer;
using Niazza.KafkaMessaging.ErrorHandling;
using Niazza.KafkaMessaging.Producer;
using Npgsql;
using OrderManager.Console.Application;
using OrderManager.Console.Bus;
using OrderManager.Domain.Aggregate;
using OrderManager.Domain.Storage;
using OrderManager.Events;
using OrderManager.Events.Converters;
using OrderManager.Infrastructure;
using OrderManager.Infrastructure.Database;
using OrderManager.Integration;
using OrderManager.Projections;
using ConverterRegistry = OrderManager.Domain.Storage.ConverterRegistry;

namespace OrderManager.Console
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        
        
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddApiExplorer();
            AddConverters(services);
            AddKafkaEvent(services);
            services.AddSingleton<ScheduleService>();
            services.AddMediatR(Assembly.GetExecutingAssembly().ExportedTypes
                    .Union(typeof(ComponentStatusHandler).Assembly.ExportedTypes).ToArray())
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }

        private void AddConverters(IServiceCollection service)
        {
            var registry = new ConverterRegistry();
            registry.Register(new ProcessedEventConverterV1toV2());
            service.AddSingleton<IConverterRegistry>(registry);
            service.AddProcessing(_configuration);
            service.AddScoped<IRepository<Order, string>, OrderRepository>();
            service.AddScoped<IEventSource<OrderSnapshotObject>, EventStore<OrderSnapshotObject>>();
            service.AddSingleton<IMessageDeserializer>(
                provider => new MessageDeserializer(typeof(IDomainEvent).Assembly
                    .GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IDomainEvent))).ToArray()));
            service.AddInfrastructure();
            service.AddProjection();
            PreparePostgresConnection(service);
        }


        private void AddKafkaEvent(IServiceCollection collection)
        {
            var config = _configuration.GetSection("KafkaConfiguration").Get<ConsumerConfiguration>();
            collection.AddKafkaProducers(new ProducerConfiguration
            {
                Servers = config.Servers,
                AckType = AckToBrokersType.ToAllBrokers,
                Idempotency = MessageIdempotency.UseIdempotent
            });

            const int oneDayAttempts = 8640;
            const double periodBetweenAttempts = 10000;

            var errorHandler = new Dictionary<ExecutionResult, ErrorHandlingConfiguration>
            {
                {
                    ExecutionResult.Failed, new ErrorHandlingConfiguration
                    {
                        RepeatHandlingType = RepeatHandlingType.RepeatScheduled,
                        Parameters = new Dictionary<string, object>
                        {
                            {
                                ErrorHandlingUtils.ErrorHandlingConstants.TimeRangeMs,
                                Enumerable.Repeat(periodBetweenAttempts, oneDayAttempts).ToArray()
                            }
                        }
                    }
                }
            };
            
            collection.AddKafkaConsumers(
                options =>
                {
                    options.Servers = config.Servers;
                    options.GroupId = config.GroupId;
                    options.IsAutocommitErrorHandling = config.IsAutocommitErrorHandling;
                    options.ManualCommitIntervalMs = config.ManualCommitIntervalMs;
                    options.Behavior = config.Behavior;
                    options.AsyncHandlingIntervalMs = config.AsyncHandlingIntervalMs;
                    options.ErrorTopicPrefix = config.ErrorTopicPrefix;
                },
                new IHandlerContainer[]
                {
                    new HandlerContainer<AddComponentMessageHandler, AddComponentMessage>(errorHandler),
                    new HandlerContainer<SendToProcessingMessageHandler, SendToProcessingMessage>(
                        new CustomizedHandlerSettings(behavior:MainConsumerBehavior.Hybrid, errorHandlerConfiguration:errorHandler)),
                });
        }
        
        public void PreparePostgresConnection(IServiceCollection services)
        {
            var host = string.Format(_configuration.GetSection("Database:Host").Value, Environment.GetEnvironmentVariable("O3_RELEASE_NAME"));
            var port = _configuration.GetSection("Database:Port").Get<int>();
            var database = _configuration.GetSection("Database:Name").Value;
            var appName = _configuration.GetSection("Database:ApplicationName").Value;

            var credentials =_configuration.GetSection($"Vault:{nameof(DbCredentials)}").Get<DbCredentials>();

            services.Configure<DbConfiguration>(
                c =>
                {
                    c.RetryCount = _configuration.GetSection("Database:RetryCount").Get<int>();
                    c.CooldownIntervalMs = _configuration.GetSection("Database:CooldownIntervalMs").Get<int>();
                    c.Host = host;
                    c.Credentials = credentials;
                    c.Port = port;
                    c.Database = database;
                    c.ApplicationName = appName;
                });

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Database = database,
                Port = port,
                Host = host,
                Password = credentials.Password,
                Username = credentials.UserId,
                ApplicationName = appName
            };

        }

        public void Configure(IApplicationBuilder app, IServiceProvider provider, IApplicationLifetime lifetime)
        {
            var starter = provider.GetService<IConsumerStarter>();
            lifetime.ApplicationStarted.Register(() => starter.RunAsync(_tokenSource.Token));
            lifetime.ApplicationStopping.Register(() => _tokenSource.Cancel());
        }
    }
}