using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using R.BooBus.RabbitMQ;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMQEventBus _azureServiceBus;

        public Worker(ILogger<Worker> logger, IRabbitMQEventBus azureServiceBus)
        {
            _logger = logger;
            _azureServiceBus = azureServiceBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               
                var message = new PublishVideoEvent();
                message.Message = "This is a more one content, of net core arquitecture series.";
                _azureServiceBus.Publish(message);

                _logger.LogInformation("message send {message} at: {time}", message.Message, DateTimeOffset.Now);

                await Task.Delay(100, stoppingToken);
            }
        }
    }                      
}
