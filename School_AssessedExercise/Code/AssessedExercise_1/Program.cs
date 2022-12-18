// See https://aka.ms/new-console-template for more information

var queue_names = new Queue<string>();

// Ask user enter 10 or more names;
Console.WriteLine("Please enter 10 more names: ");

var conuter = 0;
while (conuter <= 10)
{
    Console.WriteLine("If you meed exit or stop then pelase write 'exit':");
    var userInput = Console.ReadLine();
    if (userInput.ToLower() == "exit")
        break;

    queue_names.Enqueue(userInput);
    conuter++;
}

// Exit if queue is empty
if (queue_names.Count < 0)
    return;

// Write down mane in eueue...
Console.WriteLine("The queue of your names: ");
for (var i = 0; i < queue_names.Count; i++)
{
    Console.WriteLine(queue_names.ElementAt(i));
}