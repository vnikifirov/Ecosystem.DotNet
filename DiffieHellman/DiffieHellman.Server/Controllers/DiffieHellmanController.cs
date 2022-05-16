using DiffieHellman.Business.Models.Request;
using DiffieHellman.Business.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DiffieHellman.Server.Controllers;

[ApiController]
[Route("cryptography/diffie-hellman")]
public class DiffieHellmanController : ControllerBase
{
    private readonly Business.Services.Interfaces.IDiffieHellmanService _diffieHellman;

    public DiffieHellmanController(Business.Services.Interfaces.IDiffieHellmanService diffieHellman) => _diffieHellman = diffieHellman;

    /*  Task description
        1е еще один API Decrypt
        2е Прописать Route
        3е Создать отдельные модели WebApp модель (model) Request (string key and message) and Response (string message)
        4e Сделать Console client в нем создать класс HttpClient (WebClient is legacy). Он будет дергать ручки нашего Web Server
        5e GET Public keys for Alice and Bob by user IDs
     */

    /// <summary>
    /// Encrypt user message 
    /// </summary>
    /// <param name="publicKey">Public key of Alice or first user</param>
    /// <param name="message">Encrypting or secret message from Alice or first user</param>
    /// <returns><see cref="Encrypted message"/></returns>
    [HttpPost]
    [Route("encrypt")]
    [ProducesResponseType(typeof(ActionResult<string>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<UserMessageResult> PostEncrypt(UserMessage userMessage, CancellationToken cancellationToken)
    {
        return new UserMessageResult(Encoding.UTF8.GetString(await _diffieHellman.EncryptAsync(Encoding.UTF8.GetBytes(userMessage.PublicKey), userMessage.Message, cancellationToken)));
    }

    /// <summary>
    /// Decrypting received message
    /// </summary>
    /// <param name="publicKey">Public key of Bob or second user</param>
    /// <param name="message">Decrypting secret message from Alice or first user</param>
    /// <returns><see cref="Decrypting message"/></returns>
    [HttpPost]
    [Route("decrypt")]
    [ProducesResponseType(typeof(ActionResult<string>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]  
    public async Task<UserMessageResult> PostDecrypt(UserMessage userMessage, CancellationToken cancellationToken)
    {
        return new UserMessageResult(await _diffieHellman.DecryptAsync(Encoding.UTF8.GetBytes(userMessage.PublicKey), Encoding.UTF8.GetBytes(userMessage.Message), cancellationToken));
    }
}

