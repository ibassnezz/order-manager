using Microsoft.Extensions.DependencyInjection;
using OrderManager.Infrastructure.Repository;

namespace OrderManager.Projections
{
    public static class Composer
    {
        public static IServiceCollection AddProjection(this IServiceCollection collection)
        {
            collection.AddTransient<IOrderComponentRepository, OrderComponentRepository>();
            return collection;
        }
    }
}
