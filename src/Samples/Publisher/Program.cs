using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using R.BooBus.RabbitMQ.ConfigurationExtensions;

namespace Publisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddHostedService<Worker>();

                    services.UseRabbitMq("amqp://guest:guest@localhost", "Gabriel", nameof(PublishVideoEvent));

                    //.UseAzureServiceBus("Endpoint=sb://newsgpsmonitriip.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=LzyI2BapSPyzLU/giGQ/7UTR6s9V+Ip0a5JqrHmZ75c=")
                    //.WithTopic("rboobus")
                    //.WithSubscription("PublishVideoEvent");


                });
    }
}
