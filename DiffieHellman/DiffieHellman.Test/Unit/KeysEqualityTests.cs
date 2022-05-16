using System;
using NUnit.Framework;

namespace DiffieHellman.Test.Unit;

public class KeysEqualityTests
{
    // (g ^ a) mod p

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void KeysIsEqual_ShouldBeTrue()
    {
        int g = 3; // Usually q is small number
        int p = 17; // Usually p is large number it could be 10^300

        int alice = 6; // User Alice and her Secret key. Also the key has to be huge number 
        int bob = 15;  // User Bob and his Secret key. Also the key has to be huge number

        // It's public keys these keys can be sended by public and not security channel 
        double publicKeyAlice = Math.Pow(g, alice) % p;
        double publicKeyBob = Math.Pow(g, bob) % p;

        double genericKeyAlice = Math.Pow(publicKeyBob, alice) % p;
        double genericKeyBob = Math.Pow(publicKeyAlice, bob) % p;
        
        // ((g ^ a) mod p)^b mod p = ((g ^ b) mod p)^a mod p
        // g^a^b = g^b^a - From school 
        Assert.AreEqual(genericKeyAlice, genericKeyBob);
    }

    [Test] 
    [TestCase(3, 4)] // User Alice and Bob and their Secret keys. Also the keys have to be huge number 
    [TestCase(4, 5)]
    public void KeysIsEqual_ShouldBeTrue(int alice, int bob)
    {
        int g = 3; // Usually q is small number
        int p = 17; // Usually p is large number it could be 10^300

        // It's public key these keys can be sended by public and not security channel 
        double publicKeyAlice = Math.Pow(g, alice) % p;
        double publicKeyBob = Math.Pow(g, bob) % p;

        double genericKeyAlice = Math.Pow(publicKeyBob, alice) % p;
        double genericKeyBob = Math.Pow(publicKeyAlice, bob) % p;

        // ((g ^ a) mod p)^b mod p = ((g ^ b) mod p)^a mod p
        // g^a^b = g^b^a - From school
        // Alice and Bob have completely diff keys, but their generic keys always are equal
        Assert.AreEqual(genericKeyAlice, genericKeyBob); 
    }
}
