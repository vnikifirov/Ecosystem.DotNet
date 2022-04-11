using System;

namespace MessageLogic
{
    public interface IReceiver : IDisposable
    {
        /// <summary>
        /// Consume on message received with some action with data
        /// </summary>
        public void Consume<T>(Action<Message<T>> action)
            where T : SimpleBody;
    }
}
