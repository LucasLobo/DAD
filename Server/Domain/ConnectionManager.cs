using System;
using System.Collections.Generic;
using Utils;

namespace GStoreServer.Domain
{
    class ConnectionManager : GenericConnectionManager<Server, MasterReplicaService.MasterReplicaServiceClient>
    {
        private string selfServerId;

        // Partitions in which this server is a Master
        private readonly ISet<string> masterPartitions;

        // Partitions in which this server is a Replica
        private readonly ISet<string> replicaPartitions;
        public ConnectionManager(IDictionary<string, Server> servers, IDictionary<string, Partition> partitions, ISet<string> masterPartitions, ISet<string> replicaPartitions, string selfServerId) : base(servers, partitions)
        {
            if (string.IsNullOrWhiteSpace(selfServerId))
            {
                throw new System.ArgumentException($"'{nameof(selfServerId)}' cannot be null or whitespace");
            }

            this.masterPartitions = masterPartitions ?? throw new System.ArgumentNullException(nameof(masterPartitions));
            this.replicaPartitions = replicaPartitions ?? throw new System.ArgumentNullException(nameof(replicaPartitions));
            this.selfServerId = selfServerId;
        }

        public ISet<Server> GetPartitionReplicas(string partitionId)
        {
            Partition partition = GetPartition(partitionId);

            ISet<Server> replicas = new HashSet<Server>();
            foreach (string replicaId in partition.ReplicaSet)
            {
                try
                {
                    Server server = GetServer(replicaId);
                    replicas.Add(GetServer(replicaId));
                }
                catch (Exception e)
                {
                    // fixme
                    Console.WriteLine(e.Message);
                }
            }
            return replicas;
        }

        public ISet<Server> GetMastersOfSelfReplicaPartitions()
        {
            ISet<Server> masters = new HashSet<Server>();
            foreach (string partitionId in replicaPartitions)
            {
                Partition partition = GetPartition(partitionId);
                Server server = GetServer(partition.MasterId);
                masters.Add(server);
            }
            return masters;
        }

        public bool IsMasterForPartition(string partitionId)
        {
            return masterPartitions.Contains(partitionId);
        }

        public new void DeclareDead(string serverId)
        {
            base.DeclareDead(serverId);

            // todo need to fix masterPartitions and replicaPartitions
        }
    }
}
