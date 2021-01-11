using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using R.BooBus.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace R.BooBus.RabbitMQ
{
    public class RabbitMQEventBus : IRabbitMQEventBus
    {
        private readonly IPersistentConnection<IModel> _connection;
        private readonly IServiceProvider _provider;


        private readonly EventBusSubscriptionManager _subscriptionManager;
        private IModel _consumerChannel;
        public string _exchangeName;
        private string _queueName;

        public RabbitMQEventBus(
            IPersistentConnection<IModel> connection,
            IServiceProvider provider,
            string exchangeName = "R.Boobus",
            string queueName = "R.Boobus.Queue")
        {
            _connection = connection;
            _provider = provider;
            _exchangeName = exchangeName;

            _subscriptionManager = new EventBusSubscriptionManager();
            _subscriptionManager.OnEventRemoved += OnSubscriptionManagerEventRemoved;
            _subscriptionManager.OnEventAdded += OnSubscriptionManagerEventAdded;
            _queueName = queueName;

            _consumerChannel = RegisterMessageListener();
        }


        void OnSubscriptionManagerEventAdded(object _, string eventName)
        {
            
            using (var channel = _connection.GetModel())
            {
                channel.QueueBind(
                    queue: _queueName,
                    exchange: _exchangeName,
                    routingKey: _queueName
                    );
            }
        }

        void OnSubscriptionManagerEventRemoved(object _, string eventName)
        {
           

            using (var channel = _connection.GetModel())
            {
                channel.QueueUnbind(
                    queue: _queueName,
                    exchange: _exchangeName,
                    routingKey: _queueName
                );

                if (!_subscriptionManager.IsEmpty) return;

                _queueName = string.Empty;
                _consumerChannel.Close();
            }
        }


        public void Publish(Event @event)
        {

            using (var channel = _connection.GetModel())
            {
                var eventName = @event.GetType().Name;
                //_queueName = eventName;

                channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic);
                channel.QueueDeclare(_queueName, true, false, false, null);
                //channel.QueueBind(_queueName, _exchangeName, _queueName, null);

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: _exchangeName,
                       routingKey: _queueName,
                       basicProperties: null,
                       body: body);
            }
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            _subscriptionManager.AddSubscription<TEvent, THandler>();
        }

        public void Unsubscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            _subscriptionManager.RemoveSubscription<TEvent, THandler>();
        }


        private IModel RegisterMessageListener()
        {
            

            var channel = _connection.GetModel();
            channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic);
            channel.QueueDeclare(_queueName, true, false, false, null);
            //channel.QueueBind(_queueName, _exchangeName, _queueName, null);
            //_queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                await ProcessEvent(eventName, message);
            };

            channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = RegisterMessageListener();
            };

            return channel;
        }



        private async Task<bool> ProcessEvent(string eventName, string message)
        {
            var processed = false;

            if (_subscriptionManager.HasSubscriptions(eventName))
            {

                using (var scope = _provider.CreateScope())
                {
                    var subscriptions = _subscriptionManager.GetHandlers(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        await subscription.Handle(message, scope);
                    }
                }

                processed = true;
            }
            return processed;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _subscriptionManager.Clear();
            _consumerChannel?.Dispose();
        }
    }
}
