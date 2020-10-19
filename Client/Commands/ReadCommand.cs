using Client.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Client.Commands
{
    class ReadCommand : Command
    {
        public static int EXPECTED_ARGUMENTS = 3;

        private ConnectionManager ConnectionManager;

        public ReadCommand(ConnectionManager connectionManager) : base(false)
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

            String partitionId = arguments.ElementAt(0);
            String objectId = arguments.ElementAt(1);
            String serverId = arguments.ElementAt(2);

            try
            {
                Server server = ConnectionManager.ChooseServerForRead(partitionId, serverId);

                GStoreReadRequest gStoreReadRequest = new GStoreReadRequest()
                {
                    ObjectIdentifier = new DataObjectIdentifier
                    {
                        PartitionId = partitionId,
                        ObjectId = objectId
                    }
                };

                Console.WriteLine($"Read... {partitionId} {objectId} {serverId}");
                GStoreReadReply gStoreReadReply = await server.Stub.ReadAsync(gStoreReadRequest);

                Console.WriteLine($"PartitionId: {gStoreReadReply.Object.ObjectIdentifier.PartitionId} | ObjectId: {gStoreReadReply.Object.ObjectIdentifier.ObjectId} | Value: {gStoreReadReply.Object.Value}");
            }
            catch (ServerBindException e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
        }
    }
}
