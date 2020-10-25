using Client.Controllers;
using Client.Domain;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Client.Commands
{
    class StatusCommand : Command
    {
        public static int EXPECTED_ARGUMENTS = 0;

        private readonly ConnectionManager ConnectionManager;

        public StatusCommand(ConnectionManager connectionManager) : base(false)
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

            Console.WriteLine("Status");
            await Task.Run(() => {
                ConnectionManager.PrintPartitions();
                //TODO: Later add more stuff to print
            });
        }
    }
}
