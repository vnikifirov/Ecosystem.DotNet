// See https://aka.ms/new-console-template for more information
using DiffieHellman.Business.Models.Request;
using DiffieHellman.Business.Services.Implementation;

var client = new Consumer();

Console.WriteLine("Connect...");
Console.Write("Please enter your message: ");

var userMessage = Console.ReadLine();

Console.WriteLine("Sending...");

var publicKey = Guid.NewGuid().ToString();
var enryped = await client.EncryptAsync(new UserMessage(publicKey, userMessage), CancellationToken.None);
// Alice: Sending, Bob: Recieving
// var firstUserMsg = "Hello! I'm Alice";
// Bob: Sending, Alice: Recieving
//var secondUserMsg = "Hello, Alice! I'm Bob";
// Threory: Alice (Client #1) <- Server (Third-party) -> Bob (Client #2) - Exchanging msgs between Alice and Bob
// Steps:
// 1. Alice (Client #1) is encrypting message
// 1.1 Alice(Client #1) has to know Bob's public key
// 2. Alice (Client #1) is sending message to serve. The API POST to queue (FIFO)
// 3. Bob (Client #2) throught loop is monitoring incoming msgs. The API GET from queue (FIFO)
// 4. Bob (Client #2)  is decrypting message from Alice. 
// 4.1 Bob (Client #2) has to know Alice's public key
// Note:
// 1. Public keys these keys can be sended by public and not security channel 
// 2. Client (#1 and #2) has to cipher our messages before sending messages
var message = new UserMessage(publicKey, userMessage);


Console.Write("Message received: ");