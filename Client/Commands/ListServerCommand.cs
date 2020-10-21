using Client.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Client.Commands
{
    class ListServerCommand : Command
    {
        public static int EXPECTED_ARGUMENTS = 1;

        private ConnectionManager ConnectionManager;

        public ListServerCommand(ConnectionManager connectionManager) : base(false)
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

            string serverId = arguments.ElementAt(0);

            Console.WriteLine($"List Server: {serverId}");

            try
            {
                Server server;
                server = ConnectionManager.GetServer(serverId);
                GStoreListServerReply gStoreListServerReply = await server.Stub.ListServerAsync(new Google.Protobuf.WellKnownTypes.Empty());
                foreach (DataObjectReplica replica in gStoreListServerReply.ObjectReplicas)
                {
                    Console.WriteLine($"Partition: {replica.Object.ObjectIdentifier.PartitionId} | Server: {replica.Object.ObjectIdentifier.PartitionId} " +
                        $"| Master: {replica.IsMasterReplica} | Value: {replica.Object.Value}");
                }
            }
            catch (ServerBindException e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
        }
    }
}
