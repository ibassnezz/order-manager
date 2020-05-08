using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderManager.Integration
{
    public static class Composer
    {
        public static IServiceCollection AddProcessing(this IServiceCollection collection, IConfiguration configuration)
        {
            collection.Configure<ProcessingConfigurations>(configuration.GetSection("Processing"));
            collection.AddHttpClient<IProcessingClient, ProcessingClient>();
            collection.AddScoped<IProcessingProviderService, ProcessingProviderService>();
            return collection;
        }

    }
}
