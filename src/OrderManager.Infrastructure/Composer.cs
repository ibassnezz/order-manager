using System;
using System.Data;
using Dapper;
using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Npgsql;
using OrderManager.Domain.Storage;
using OrderManager.Infrastructure.Database;
using OrderManager.Infrastructure.Repository;

namespace OrderManager.Infrastructure
{
    public static class Composer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection collection)
        {
            FluentMapper.Initialize(
                config =>
                {
                    config.AddMap(new DataWithVersionMap());
                    config.AddMap(new OrderComponentMap());
                });

            SqlMapper.AddTypeHandler(new JObjectTypeHandler());
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet(jsonbClrTypes: new Type[] {typeof(JObject)});
            collection.AddScoped<IEventDataLayer, EventDataLayer>();
            collection.AddSingleton<IDatabaseConnectionFactory, DatabaseConnectionFactory>();

            return collection;
        }
    }

    public class JObjectTypeHandler : SqlMapper.TypeHandler<JObject>
    {
        public override void SetValue(IDbDataParameter parameter, JObject value)
        {
            if (value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }
        }

        public override JObject Parse(object value)
        {
            if (value is null || value == DBNull.Value)
            {
                return null;
            }

            return JObject.Parse(value as string);
        }
    }
}
