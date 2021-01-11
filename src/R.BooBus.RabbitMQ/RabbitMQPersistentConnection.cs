using RabbitMQ.Client;
using System;

namespace R.BooBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IPersistentConnection<IModel>, IDisposable
    {
        private readonly string _connectionString;
        private IConnection _connection;

        public RabbitMQPersistentConnection(string connectionString)
        {
            _connectionString = connectionString;
            var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };
            _connection = factory.CreateConnection();

        }

        

        public IModel GetModel()
        {
            
            if (!_connection.IsOpen) 
            {
                var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };
                _connection = factory.CreateConnection();
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
