using System;
using System.Collections.Generic;
using Grpc.Net.Client;

namespace Client.Domain
{
    class ConnectionManager
    {
        private readonly IDictionary<string, Server> serverSet;
        private readonly IDictionary<string, Partition> partitionSet;

        private Server attachedServer;

        public ConnectionManager()
        {
            serverSet = new Dictionary<string, Server>();
            partitionSet = new Dictionary<string, Partition>();
            CreatePartitions();
        }

        private void CreatePartitions()
        {

            int serverNumber = 5;
            int partitionSize = 3;

            for (int i = 1; i <= serverNumber; i++)
            {
                string serverId = "s-" + i;
                int port = 8080 + i;
                string address = "http://localhost:" + port;
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                GStoreService.GStoreServiceClient client = new GStoreService.GStoreServiceClient(channel);
                serverSet.Add(serverId, new Server(serverId, client));
            }

            for (int i = 1; i <= serverNumber; i++)
            {
                string masterId = "s-" + i;
                Server master = serverSet[masterId];

                ISet<Server> partitionServerSet = new HashSet<Server>();

                for (int j = 0; j < partitionSize; j++)
                {
                    string serverId = "s-" + ((i + j - 1) % serverNumber + 1);
                    Server server = serverSet[serverId];
                    partitionServerSet.Add(server);
                }

                string partitionId = "part-" + i;
                Partition partition = new Partition(partitionId, master, partitionServerSet);
                partitionSet.Add(partitionId, partition);
            }
        }

        public void PrintPartitions()
        {
            foreach (KeyValuePair<string, Partition> entry in partitionSet)
            {
                entry.Value.Print();
                Console.WriteLine();
            }
        }


        public Server ChooseServerForRead(string partitionId, string serverId)
        {
            Partition partition = GetPartition(partitionId);

            // No server currently attached
            if (attachedServer == null)
            {
                if (serverId.Equals("-1"))
                {
                    throw new ServerBindException("Not attached to any server and no default server is provided.");
                }
                else if (!partition.Contains(serverId))
                {
                    throw new ServerBindException("Default server '" + serverId + "' is not part of partition '" + partitionId + "'.");
                }
                else
                {
                    attachedServer = GetServer(serverId);
                }
            }

            else if (!partition.Contains(attachedServer.Id))
            {
                if (serverId.Equals("-1"))
                {
                    throw new ServerBindException("Attached server '" + attachedServer.Id + "' is not part of the partition '" + partitionId + "' and no default server is provided.");
                }
                else if (!partition.Contains(serverId))
                {
                    throw new ServerBindException("Default server '" + serverId + "' is not part of partition '" + partitionId + "'.");
                }
                else
                {
                    attachedServer = GetServer(serverId);
                }
            }

            return attachedServer;
        }

        public Server ChooseServerForWrite(string partitionId)
        {
            Partition partition = GetPartition(partitionId);

            // should it attach itself?
            // attachedServerId = partition.MasterId;
            return partition.Master;
        }

        private Partition GetPartition(string partitionId)
        {
            partitionSet.TryGetValue(partitionId, out Partition partition);
            if (partition == null)
            {
                throw new ServerBindException("Partition '" + partitionId + "' not found.");
            }
            return partition;
        }

        public Server GetServer(string serverId)
        {
            serverSet.TryGetValue(serverId, out Server server);
            if (server == null)
            {
                throw new ServerBindException("Server '" + serverId + "' not found.");

            }
            return server;
        }

    }

    [Serializable]
    public class ServerBindException : Exception
    {
        public ServerBindException()
        { }

        public ServerBindException(string message)
            : base(message)
        { }

        public ServerBindException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
