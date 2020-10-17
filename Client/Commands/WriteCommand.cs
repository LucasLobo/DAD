using Client.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Client.Commands
{
    class WriteCommand : Command
    {
        public static int EXPECTED_ARGUMENTS = 3;

        private ConnectionManager ConnectionManager;

        public WriteCommand(ConnectionManager connectionManager) : base(false)
        {
            ConnectionManager = connectionManager ?? throw new ArgumentNullException("ConnectionManager cannot be null.");
        }

        public override async Task ExecuteAsync(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                Console.WriteLine("Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }
            Console.WriteLine($"Write... {arguments.ElementAt(0)}");
            await Task.Delay(1000);
            Console.WriteLine($"Write End... {arguments.ElementAt(0)}");
        }
    }
}
