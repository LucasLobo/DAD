using Client.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using Client.Controllers;

namespace Client.Commands
{

    class ListGlobalCommand : Command
    {
        public static int EXPECTED_ARGUMENTS = 0;

        private readonly ConnectionManager ConnectionManager;

        public ListGlobalCommand(ConnectionManager connectionManager) : base(false)
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
            Console.WriteLine("listGlobal");

            HashSet<GStoreObjectIdentifier> gStoreObjectIdentifiers = await ListGlobalController.Execute(ConnectionManager);

            foreach (GStoreObjectIdentifier objectId in gStoreObjectIdentifiers)
            {
                Console.WriteLine($"=> {objectId.PartitionId}, {objectId.ObjectId}");
            }
        }
    }
}
