using FastPaceTransferTest2022.Api.Configurations;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FastPaceTransferTest2022.Api.ServiceExtensions
{
    public static class RedisExtensions
    {
        public static void InitializeRedis(this IServiceCollection services, RedisConfiguration redisConfig)
        {
            services.Configure<RedisConfiguration>(c =>
            {
                c.Url = redisConfig.Url;
            });
            
            var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { redisConfig.Url }
            });

            services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
        }
    }
}