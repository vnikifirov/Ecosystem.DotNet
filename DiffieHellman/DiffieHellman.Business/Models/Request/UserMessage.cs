using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffieHellman.Business.Models.Request
{
    /// <summary>
    /// User message Alice or Bob
    /// </summary>
    public sealed class UserMessage
    {
        public string PublicKey { get; set; }
        public string Message { get; set; }

        public UserMessage() {}

        public UserMessage(string publicKey, string message)
        {
            PublicKey = publicKey;
            Message = message;
        }
    }
}
