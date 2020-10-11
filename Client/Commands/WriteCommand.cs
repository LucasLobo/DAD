using System;
using System.Collections.Generic;
using Utils;

namespace Client.Commands
{
    class WriteCommand : Command
    {
        public static int EXPECTED_ARGUMENTS = 3;

        public override void Execute(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                Console.WriteLine("Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }
            Console.WriteLine("Processing...");
        }
    }
}
