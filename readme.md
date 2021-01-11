# R.BooBus.RabiitMq [![Build Status](https://travis-ci.com/rnascimento10/R.BooBus.svg?branch=master)](https://travis-ci.com/rnascimento10/R.BooBus) [![NuGet Package](https://img.shields.io/nuget/v/R.Boobus.AzureServiceBus.svg)](https://www.nuget.org/packages/R.BooBus.AzureServiceBus)

A smart and simple library for pub sub implementtion to RabbitMq

### Package Manager Console

```
Install-Package R.BooBus.RabiitMq
```

### .NET Core CLI

```
dotnet add package R.BooBus.RabiitMq
```

## Usage

1 - In the publish process, register the service in net core DI container with the fluent built in api like this:

```csharp
      //A instace of ServiceCollection 
      services.UseRabbitMq("amqp://guest:guest@localhost", "Gabriel", nameof(PublishVideoEvent));

```
2 - Create a custom event that inherits from the Event Class of the R.Boobus.Core namespace
```csharp

 public class MyEvent : Event
 {
 }
```
3 - We are now ready to publish the event on the bus.

```csharp
 public class SomeClass 
 {
        private readonly IRabbitMQEventBus _bus;

        public SomeClass(IRabbitMQEventBus bus)
        {
            _bus = azureServiceBus;
        }

        protected async Task runAsync()
        {
             var message = new MyEvent();             
             _bus.Publish(message);
        }
    }
```
5 - In the subscriber process, register the service in net core DI container with the fluent built in api like this:

```csharp
      //A instace of ServiceCollection 
       services.UseRabbitMq("amqp://guest:guest@localhost", "Gabriel", nameof(PublishVideoEvent));
```

6 - Subscribe to start receiving your message or unsubscribe to stop receiving then.

```csharp
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
```


7 - Create the same event created on publish process that inherits from the Event Class of the R.Boobus.Core namespace
```csharp

 public class MyEvent : Event
 {
 }
```
8 - Create the event handler that implement IEventHandler<TEvent> from the R.Boobus.Core namespace
```csharp
 public class HelloHandler : IEventHandler<MyEvent>
    {
        private ILogger<HelloHandler> _logger;

        public HelloHandler(ILogger<HelloHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(MyEvent @event)
        {
            _logger.LogInformation("Get Event from  {@event} at {time}", @event.ToString(), DateTimeOffset.Now);

            return Task.FromResult(true);
        }
    }
```


