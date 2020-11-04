using Utils;
using System.Collections.Generic;
using System;

namespace Client.Domain
{
    class ConnectionManager : GenericConnectionManager<Server, GStoreService.GStoreServiceClient>
    {

        private Server attachedServer;

        public ConnectionManager(IDictionary<string, Server> servers, IDictionary<string, Partition> partitions) : base(servers, partitions)
        {
        }


        public Server ChooseServerForRead(string partitionId, string serverId)
        {
            _ = GetPartition(partitionId); // throws exception if partition doesn't exist

            if (attachedServer != null && PartitionContainsAlive(partitionId, attachedServer.Id))
            {
                return attachedServer;
            }

            Server defaultServer = null;
            if (serverId != "-1")
            {
                defaultServer = GetServer(serverId);
            }

            if (defaultServer != null && PartitionContainsAlive(partitionId, defaultServer.Id))
            {
                attachedServer = defaultServer;
                return attachedServer;
            }

            throw new ServerBindException($"No valid attached or default server. Partition: {partitionId} | AttachedServer: ({attachedServer}) | DefaultServer: {serverId}");
            // Choose a valid server if needed
        }


        // Throws exception if partitionId is not valid or if master server is dead
        public Server ChooseServerForWrite(string partitionId)
        {
            Partition partition = GetPartition(partitionId);
            attachedServer = GetAliveServer(partition.MasterId);
            return attachedServer;
        }
    }
}
