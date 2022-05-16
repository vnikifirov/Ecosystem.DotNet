using DiffieHellman.Business.Models.Request;
using DiffieHellman.Business.Models.Response;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiffieHellman.Business.Services.Implementation
{
    public class Consumer
    {
        private string URL { get; set; } = "http://localhost:5207/cryptography/diffie-hellman/";
        private string Send { get; set; } = "endrypt";
        private string Recieve { get; set; } = "decrypt";

        public Consumer() { }

        public Consumer(string url, string send, string recieve)
        {
            URL = url;
            Send = send;
            Recieve = recieve;
        }

        /// <inheritdoc/>
        public async Task<UserMessageResult> EncryptAsync(UserMessage userMessage, CancellationToken cancellationToken)
        {
            // Form URL
            var uri = new Uri(string.Format("{0}/{1}", URL, Send));
            var json = JsonConvert.SerializeObject(userMessage);

            // Create client
            using var web = new HttpClient();
            var response = await web.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);

            // Get encrypted message
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var encryptedMsg = JsonConvert.DeserializeObject<UserMessageResult>(responseJson);
            return encryptedMsg;
        }

        /// <inheritdoc/>
        public async Task<UserMessageResult> DecryptAsync(UserMessage encryptedMessage, CancellationToken cancellationToken)
        {
            // Form URL
            var uri = new Uri(string.Format("{0}/{1}", URL, Recieve));
            var json = JsonConvert.SerializeObject(encryptedMessage);

            // Create client
            using var web = new HttpClient();
            var response = await web.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);

            // Get encrypted message
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var encryptedMsg = JsonConvert.DeserializeObject<UserMessageResult>(responseJson);
            return encryptedMsg;
        }
    }
}
