using System.Threading.Tasks;
using NUnit.Framework;

namespace DiffieHellman.Test.Unit
{
	public class DiffieHellmanTests
	{
        private Business.Services.Interfaces.IDiffieHellmanService _diffieHellmanAlice { get; set; }
		private Business.Services.Interfaces.IDiffieHellmanService _diffieHellmanBob { get; set; }
		private string _text { get; set; }

        [SetUp]
		public void Setup()
		{
			_diffieHellmanAlice = new Business.Services.Implementation.DiffieHellmanService();
			_diffieHellmanBob = new Business.Services.Implementation.DiffieHellmanService();
			_text = "My random text";
		}

		[Test]
		public async Task EncryptDecrypt_ShouldBeTrue()
		{
			var encryptedMsg = await _diffieHellmanAlice.EncryptAsync(_diffieHellmanBob.PublicKey, _text, new System.Threading.CancellationToken());
			var decryptedMsg = await _diffieHellmanBob.DecryptAsync(_diffieHellmanAlice.PublicKey, encryptedMsg, new System.Threading.CancellationToken());

			Assert.AreEqual(_text, decryptedMsg);
		}

		[TearDown]
		public void TearDown()
		{
			_diffieHellmanAlice.Dispose();
			_diffieHellmanBob.Dispose();
		}
	}
}