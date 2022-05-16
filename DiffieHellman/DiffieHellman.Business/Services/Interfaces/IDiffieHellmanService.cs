using System;
using System.Collections.Generic;
using System.Linq;
namespace DiffieHellman.Business.Services.Interfaces
{
    /// <summary>
    /// Encrypt and Decrypt message by Diffie Hellman Algorithms
    /// </summary>
    public interface IDiffieHellmanService: IDisposable
    {
        /// <summary>
        /// Public key
        /// </summary>
        byte[] PublicKey { get; }

        /// <summary>
        /// Encrypting Alice or user message
        /// </summary>
        /// <param name="publicKey">Public key of Alice or first user</param>
        /// <param name="secretMessage">Encrypting or secret message from Alice or first user</param>
        /// <returns>Encrypted message</returns>
        Task<byte[]> EncryptAsync(byte[] publicKey, string secretMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Decrypting received message
        /// </summary>
        /// <param name="publicKey">Public key of Bob or secound user</param>
        /// <param name="encryptedMessage">Received message from Alice or first user</param>
        /// <returns>Decrypted message</returns>
        Task<string> DecryptAsync(byte[] publicKey, byte[] encryptedMessage, CancellationToken cancellationToken);
    }
}
