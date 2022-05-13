using System;
using NUnit.Framework;

namespace DiffieHellman.Test.Unit
{
	public class DiffieHellmanTests
	{
        private Business.DiffieHellman _diffieHellmanAlice { get; set; }
		private Business.DiffieHellman _diffieHellmanBob { get; set; }
		private string _text { get; set; }

        [SetUp]
		public void Setup()
		{
			_diffieHellmanAlice = new Business.DiffieHellman();
			_diffieHellmanBob = new Business.DiffieHellman();
			_text = "My random text";
		}

		[Test]
		public void EncryptDecrypt_ShouldBeTrue()
		{
			var encryptedMsg = _diffieHellmanAlice.Encrypt(_diffieHellmanBob.PublicKey, _text);
			var decryptedMsg = _diffieHellmanBob.Decrypt(_diffieHellmanAlice.PublicKey, encryptedMsg);

			Assert.AreEqual(_text, decryptedMsg);
		}
	}
}