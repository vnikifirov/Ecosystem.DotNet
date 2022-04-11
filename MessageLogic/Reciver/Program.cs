using System;
using MessageLogic;

namespace Reciver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Print routing name: ");

            Console.Write("Enter key: ");

            // Read routing
            var routingKey = Console.ReadLine();

            // Create receiver
            var exchangeName = "exchangeTest";
            using IReceiver receiver = new Receiver(exchangeName, routingKey, name: "admin", pass: "admin");

            Console.WriteLine("[*] Waiting for messages.");

            // action to do work with message
            Action<Message<Body>> DoWorkWithData = m => Console.WriteLine($"Receive to { m.RoutingKey } at { m.Date }: { m?.Body?.Text }");

            // Consume to receive messages
            receiver.Consume<Body>(DoWorkWithData);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
