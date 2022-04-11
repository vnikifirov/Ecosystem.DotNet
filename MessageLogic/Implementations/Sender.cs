using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

namespace MessageLogic
{
    /// <summary>
    /// Sender of the messages
    /// </summary>
    public class Sender : Rabbit, ISender
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Sender
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        public Sender(string exchange, ILogger? logger = null,
                        string host = "localhost", string name = "guest", string pass = "guest", string vhost = "/")
                        : base(exchange, logger, host, name, pass, vhost)
        {
            _logger = logger != null ? logger : Logger.Create<Sender>();

            CreateExchange();
        }

        void CreateExchange()
        { 
            // base arguments of exchange
            var arguments = new Dictionary<string, object>
            {
                { "x-message-ttl", 100_000 }, // message is alive
                { "x-expires", 100_000 }, // very dengerous // exchange is alive
                { "x-max-length", 100 } // max messages in queue
            };

            // create exchange with som type
            _channel.ExchangeDeclare(exchange: _exchange, 
                                     type: ExchangeType.Topic, // Topic - messages by routing template
                                     //ExchangeType.Fanout, // Fanout - messages for all
                                     //ExchangeType.Direct, // Direct - messages by routing key
                                     arguments: arguments);
        }

        /// <inheritdoc/>
        public void Send<T>(Message<T> message)
            where T : SimpleBody
        {
            try
            {
                // publish message in exchange by routing
                _channel.BasicPublish(exchange: _exchange,
                                      routingKey: message.RoutingKey,
                                      basicProperties: null,
                                      body: message.ToBytes());
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                throw;
            }
        }
    }
}
