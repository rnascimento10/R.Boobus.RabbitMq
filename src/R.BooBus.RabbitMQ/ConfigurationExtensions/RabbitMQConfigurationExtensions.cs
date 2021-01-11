using Microsoft.Extensions.DependencyInjection;

namespace R.BooBus.RabbitMQ.ConfigurationExtensions
{
    public static class RabbitMQConfigurationExtensions
    {
        public static IServiceCollection UseRabbitMq(this IServiceCollection services, string connectionString, string exchangeName = "R.Boobus", string queueName = "R.Boobus.Queue")
        {
            var serviceProvider = services.BuildServiceProvider();

            services.AddTransient<IRabbitMQEventBus, RabbitMQEventBus>(x => new RabbitMQEventBus(new RabbitMQPersistentConnection(connectionString), serviceProvider, exchangeName, queueName));
            return services;


        }

    }
}
