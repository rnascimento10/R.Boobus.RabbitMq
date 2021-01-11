using Microsoft.Extensions.Hosting;
using R.BooBus.RabbitMQ;
using Subscriber.Handlers;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriber
{
    public class Worker : BackgroundService
    {
        private IRabbitMQEventBus _bus;

        public Worker(IRabbitMQEventBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            _bus.Subscribe<PublishVideoEvent, PublishedVideoEventHandler>();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _bus.Unsubscribe<PublishVideoEvent, PublishedVideoEventHandler>();
            return base.StopAsync(cancellationToken);
        }
    }
}
