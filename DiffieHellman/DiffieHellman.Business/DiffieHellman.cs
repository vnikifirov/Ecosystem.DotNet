using System;
using System.Security.Cryptography;
using System.Text;

namespace DiffieHellman.Business
{
	public class DiffieHellman
	{
		private ECDiffieHellmanCng _ECDiffieHellmanCng;

        /// <summary>
        /// To Encryp message, but you can use RSA instead of AES
        /// </summary>
        private static Aes aes = Aes.Create();

        public byte[] PublicKey { get; set; }

        public DiffieHellman()
		{
			_ECDiffieHellmanCng = new ECDiffieHellmanCng()
			{
				KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
				HashAlgorithm = CngAlgorithm.Sha256
			};
		}

        /// <summary>
        /// Encrypting Alice or user message
        /// </summary>
        /// <param name="publicKey">Public key of Alice or first user</param>
        /// <param name="secretMessage">Encrypting or secret message from Alice or first user</param>
        /// <returns>Encrypted message</returns>
        public byte[] Encrypt(byte[] publicKey, string secretMessage)
        {
            byte[] encryptedMessage;
            var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
            var derivedKey = _ECDiffieHellmanCng.DeriveKeyMaterial(key); // "Common secret"

            aes.Key = derivedKey;

            using (var cipherText = new MemoryStream())
            {
                using (var encryptor = aes.CreateEncryptor())
                {
                    using (var cryptoStream = new CryptoStream(cipherText, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] ciphertextMessage = Encoding.UTF8.GetBytes(secretMessage);
                        cryptoStream.Write(ciphertextMessage, 0, ciphertextMessage.Length);
                    }
                }

                encryptedMessage = cipherText.ToArray();
            }

            return encryptedMessage;
        }

        /// <summary>
        /// Decrypting received message
        /// </summary>
        /// <param name="publicKey">Public key of Bob or secound user</param>
        /// <param name="encryptedMessage">Received message from Alice or first user</param>
        /// <returns>Decrypted message</returns>
        public string Decrypt(byte[] publicKey, byte[] encryptedMessage)
        {
            string decryptedMessage;
            var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
            var derivedKey = _ECDiffieHellmanCng.DeriveKeyMaterial(key);

            aes.Key = derivedKey;

            using (var plainText = new MemoryStream())
            {
                using (var decryptor = aes.CreateDecryptor())
                {
                    using (var cryptoStream = new CryptoStream(plainText, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedMessage, 0, encryptedMessage.Length);
                    }
                }

                decryptedMessage = Encoding.UTF8.GetString(plainText.ToArray());
            }

            return decryptedMessage;
        }
    }
}

