using Utils;
using System.Collections.Generic;

namespace Client.Domain
{
    class ConnectionManager : GenericConnectionManager<Server, GStoreService.GStoreServiceClient>
    {

        private string attachedServerId;

        public ConnectionManager(IDictionary<string, Server> servers, IDictionary<string, Partition> partitions) : base(servers, partitions)
        {
        }

        public Server ChooseServerForRead(string partitionId, string serverId)
        {
            Partition partition = GetPartition(partitionId);

            if (attachedServerId == null)
            {
                if (serverId == "-1")
                {
                    throw new ServerBindException("Not attached to any server and no default server is provided.");
                }
                else if (!partition.Contains(serverId))
                {
                    throw new ServerBindException("Default server '" + serverId + "' is not part of partition '" + partitionId + "'.");
                }
                else
                {
                    attachedServerId = serverId;
                }
            }

            else if (!partition.Contains(attachedServerId))
            {
                if (serverId == "-1")
                {
                    throw new ServerBindException("Attached server '" + attachedServerId + "' is not part of the partition '" + partitionId + "' and no default server is provided.");
                }
                else if (!partition.Contains(serverId))
                {
                    throw new ServerBindException("Default server '" + serverId + "' is not part of partition '" + partitionId + "'.");
                }
                else
                {
                    attachedServerId = serverId;
                }
            }

            return GetServer(attachedServerId);
        }

        public Server ChooseServerForWrite(string partitionId)
        {
            Partition partition = GetPartition(partitionId);
            return GetServer(partition.MasterId);
        }
    }
}
