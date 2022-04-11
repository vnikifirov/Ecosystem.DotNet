using System;
using MessageLogic;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create sender
            var exchangeName = "exchangeTest";
            using ISender sender = new MessageLogic.Sender(exchangeName, name: "admin", pass: "admin");

            Console.WriteLine("Press [enter] to exit.");

            while (true)
            {
                Console.Write("Enter key-message: ");

                // Read key-message
                string? input = Console.ReadLine();
                if (input == null || input == "")
                    break;
                var param = input.Split('-');

                // params of message
                var routingKey = param[0];
                var body = new FullBody(666, param[1]);

                // Create message
                var message = new Message<FullBody>(routingKey, body);

                // Send message
                sender.Send(message);

                // Log to sender console
                Console.WriteLine($"[x] Sent to { message.RoutingKey } at { message.Date }: { message?.Body?.Text }");
            }
        }
    }
}
