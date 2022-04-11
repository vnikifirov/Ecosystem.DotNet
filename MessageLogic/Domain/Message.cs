using System;
using Newtonsoft.Json;
using System.Text;

namespace MessageLogic
{
    /// <summary>
    /// Message
    /// </summary>
    /// <typeparam name="T">Data</typeparam>
    public class Message<T>
        where T : SimpleBody
    {
        /// <summary>
        /// Need for json serializer
        /// </summary>
        public Message() { }

        /// <summary>
        /// New message
        /// </summary>
        /// <param name="body"></param>
        public Message(string routingKey, T body)
        {
            RoutingKey = routingKey;
            Body = body;
        }

        /// <summary>
        /// Deserialize existing message
        /// </summary>
        public Message(byte[] bytes)
        {
            var deserialized = Encoding.UTF8.GetString(bytes);
            var message = JsonConvert.DeserializeObject<Message<T>>(deserialized);
            RoutingKey = message!.RoutingKey;
            Body = message.Body;
        }

        /// <summary>
        /// Routing in the queue
        /// </summary>
        public string RoutingKey { get; set; } = "*";

        /// <summary>
        /// Message date
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Message data
        /// </summary>
        public T? Body { get; set; }

        /// <summary>
        /// Serialize
        /// </summary>
        public virtual byte[] ToBytes()
        {
            var stringMessage = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(stringMessage);
        }
    }
}