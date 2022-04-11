using System;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageLogic
{
    /// <summary>
    /// Receiver of the messages
    /// </summary>
    public class Receiver : Rabbit, IReceiver
    {
        private readonly string _queueName = string.Empty;
        private readonly string _routingKey = string.Empty;
        private readonly ILogger _logger;

        /// <summary>
        /// Receiver
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Routing</param>
        public Receiver(string exchange, string? routingKey, ILogger? logger = null, 
                            string host = "localhost", string name = "guest", string pass = "guest", string vhost = "/")
            : base(exchange, logger, host, name, pass, vhost)
        {
            _routingKey = string.IsNullOrEmpty(routingKey)
                                ? "*" // template for all messages 
                                : routingKey;

            _queueName = _exchange + ":" + _routingKey;

            _logger = logger != null ? logger : Logger.Create<Receiver>();

            CreateQueueAndBind();
        }

        void CreateQueueAndBind()
        {
            // create queue
            _channel.QueueDeclare(queue: _queueName, 
                                  durable: true, // restart after RabbitMq restart
                                  exclusive: false, // do not drop queue
                                  autoDelete: false); // do not drop messages

            // bind queue to exchange with key
            _channel.QueueBind(queue: _queueName, 
                               exchange: _exchange, 
                               routingKey: _routingKey);
        }

        /// <inheritdoc/>
        public void Consume<T>(Action<Message<T>> DoWorkWithData)
            where T : SimpleBody
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (model, ea) =>
            {
                try
                {
                    // deserialize existing message
                    var message = new Message<T>(ea.Body.ToArray());

                    try
                    {
                        // do some logic in action, ex write to console
                        DoWorkWithData(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, ex.Message);
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex.Message);
                    throw;
                }
                finally
                {
                    // message is delivered
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            // consume on queue
            _channel.BasicConsume(queue: _queueName, 
                                  autoAck: false, // no auto-deliver
                                  consumer: consumer);
        }
    }
}
