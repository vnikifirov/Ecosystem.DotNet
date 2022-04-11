using System;

namespace MessageLogic
{
    public interface ISender : IDisposable
    {
        /// <summary>
        /// Send message with data
        /// </summary>
        public void Send<T>(Message<T> message)
            where T : SimpleBody;
    }
}
