using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffieHellman.Business.Models.Response
{
    /// <summary>
    /// Decrypted user message Alice or Bob
    /// </summary>
    public class UserMessageResult
    {
        public string Message { get; set; }

        public UserMessageResult(string message) => Message = message;
    }
}
