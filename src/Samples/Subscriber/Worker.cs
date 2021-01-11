using Microsoft.Extensions.Hosting;
using R.BooBus.AzureServiceBus;
using R.BooBus.RabbitMQ;
using Subscriber.Handlers;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriber
{
    public class Worker : BackgroundService
    {
        private IRabbitMQEventBus _azureServiceBus;

        public Worker(IRabbitMQEventBus azureServiceBus)
        {
            _azureServiceBus = azureServiceBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            _azureServiceBus.Subscribe<PublishVideoEvent, PublishedVideoEventHandler>();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _azureServiceBus.Unsubscribe<PublishVideoEvent, PublishedVideoEventHandler>();
            return base.StopAsync(cancellationToken);
        }
    }
}
