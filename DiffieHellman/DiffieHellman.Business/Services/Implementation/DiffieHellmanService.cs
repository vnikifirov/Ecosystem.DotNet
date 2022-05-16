using DiffieHellman.Business.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace DiffieHellman.Business.Services.Implementation
{
    /// <inheritdoc/>
    public class DiffieHellmanService: IDiffieHellmanService
	{
		private ECDiffieHellmanCng _ECDiffieHellmanCng;

        /// <summary>
        /// To Encryp message, but you can use RSA instead of AES
        /// </summary>
        private static Aes _aes = Aes.Create();

        public byte[] PublicKey
        {
            get
            {
                return _ECDiffieHellmanCng.PublicKey.ToByteArray();
            }
        }

        public DiffieHellmanService()
		{
			_ECDiffieHellmanCng = new ECDiffieHellmanCng()
			{
				KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
				HashAlgorithm = CngAlgorithm.Sha256
			};
		}

        /// <inheritdoc/>
        public async Task<byte[]> EncryptAsync(byte[] publicKey, string secretMessage, CancellationToken cancellationToken)
        {
            byte[] encryptedMessage;
            var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
            var derivedKey = _ECDiffieHellmanCng.DeriveKeyMaterial(key); // "Common secret"

            _aes.Key = derivedKey;

            using (var cipherText = new MemoryStream())
            {
                using (var encryptor = _aes.CreateEncryptor())
                {
                    using (var cryptoStream = new CryptoStream(cipherText, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] ciphertextMessage = Encoding.UTF8.GetBytes(secretMessage);
                        await cryptoStream.WriteAsync(ciphertextMessage, 0, ciphertextMessage.Length, cancellationToken);
                    }
                }

                encryptedMessage = cipherText.ToArray();
            }

            return encryptedMessage;
        }

        /// <inheritdoc/>
        public async Task<string> DecryptAsync(byte[] publicKey, byte[] encryptedMessage, CancellationToken cancellationToken)
        {
            string decryptedMessage;
            var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
            var derivedKey = _ECDiffieHellmanCng.DeriveKeyMaterial(key);

            _aes.Key = derivedKey;

            using (var plainText = new MemoryStream())
            {
                using (var decryptor = _aes.CreateDecryptor())
                {
                    using (var cryptoStream = new CryptoStream(plainText, decryptor, CryptoStreamMode.Write))
                    {
                        await cryptoStream.WriteAsync(encryptedMessage, 0, encryptedMessage.Length, cancellationToken);
                    }
                }

                decryptedMessage = Encoding.UTF8.GetString(plainText.ToArray());
            }

            return decryptedMessage;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_aes != null)
                    _aes.Dispose();

                if (_ECDiffieHellmanCng != null)
                    _ECDiffieHellmanCng.Dispose();
            }
        }
    }
}

