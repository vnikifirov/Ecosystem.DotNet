using System;
using System.Collections.Generic;

namespace BalanceOfParentheses
{
    class Program
    {
        static void Main(string[] args)
        {
            string userInput = "(";

            var balancer = new Balancer(userInput);
            if (balancer.IsValid())
            {
                Console.WriteLine("Ok");
            }
            else
            {
                Console.WriteLine("No");
            }

            Console.ReadLine();
        }
    }

    public class Balancer
    {
        private Stack<char> Stack = new Stack<char>();
        public string UserInput { get; private set; }

        public Balancer(string userInput)
        {
            UserInput = userInput; // I/O
        }

        public bool IsValid()
        {
            if (String.IsNullOrWhiteSpace(UserInput))
            {
                return true;
            }

            foreach (var c in UserInput)
            {
                if (c == '(')
                {
                    Stack.Push(c);
                    continue;
                }

                if (c == ')')
                {
                    if (Stack.TryPop(out char r))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return Stack.Count == 0;
        }
    }
}
