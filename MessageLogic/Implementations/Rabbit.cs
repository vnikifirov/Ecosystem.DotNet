using Microsoft.Extensions.Logging;
using System;
using RabbitMQ.Client;

namespace MessageLogic
{
    /// <summary>
    /// Base config of RabbitMq
    /// </summary>
    public abstract class Rabbit : IDisposable
    {
        protected readonly string _exchange;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        private ILogger _logger;

        /// <summary>
        /// Base RabbitMq channel and connection configure
        /// </summary>
        protected Rabbit(string exchange, ILogger? logger = null, 
                            string host = "localhost", string name = "guest", string pass = "guest", string vhost = "/")
        {
            _exchange = exchange;

            _logger = logger != null ? logger : Logger.Create<Rabbit>();

            var factory = new ConnectionFactory()
            {
                HostName = host,
                UserName = name,
                Password = pass,
                VirtualHost = vhost
            };

            try
            {
                _connection = factory.CreateConnection();

                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message);
                throw;
            }
        }

        // Dispose connection and channel
        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();

            if (_channel != null)
                _channel.Dispose();
        }
    }
}
