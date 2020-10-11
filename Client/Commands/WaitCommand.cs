using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Utils;

namespace Client.Commands
{
    class WaitCommand : Command
    {
        public static int EXPECTED_ARGUMENTS = 1;
        public override void Execute(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                Console.WriteLine("Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            try
            {
                int milliseconds = Int32.Parse(arguments.ElementAt(0));
                Console.WriteLine("Sleeping...");
                Thread.Sleep(milliseconds);
            }
            catch (FormatException)
            {
                Console.WriteLine("Argument must be of type int.");
                return;
            }

        }
    }
}
