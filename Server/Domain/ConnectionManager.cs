using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace GStoreServer.Domain
{
    class ConnectionManager : GenericConnectionManager<Server, MasterReplicaService.MasterReplicaServiceClient>
    {
        private readonly string selfServerId;

        // Partitions in which this server is a Master
        private readonly ISet<string> masterPartitions;

        // Partitions in which this server is a Replica
        private readonly ISet<string> replicaPartitions;
        public ConnectionManager(IDictionary<string, Server> servers, IDictionary<string, Partition> partitions, string selfServerId) : base(servers, partitions)
        {
            if (string.IsNullOrWhiteSpace(selfServerId))
            {
                throw new ArgumentException($"'{selfServerId}' cannot be null or whitespace", nameof(selfServerId));
            }

            this.selfServerId = selfServerId;

            masterPartitions = new HashSet<string>();
            replicaPartitions = new HashSet<string>();
            foreach (Partition partition in partitions.Values)
            {
                if (partition.MasterId == selfServerId) masterPartitions.Add(partition.Id);
                else if (partition.ReplicaSet.Contains(selfServerId)) replicaPartitions.Add(partition.Id);
            }

            DeclareDead("s-1");
        }

        public ISet<Server> GetMastersOfPartitionsWhereSelfReplica()
        {
            ISet<Server> masters = new HashSet<Server>();
            foreach (string partitionId in replicaPartitions)
            {
                Partition partition = GetPartition(partitionId);
                Server master = GetServer(partition.MasterId);
                masters.Add(master);
            }
            return masters;
        }

        public bool IsMasterForPartition(string partitionId)
        {
            return masterPartitions.Contains(partitionId);
        }

        public new void DeclareDead(string deadServerId)
        {
            if (deadServerId == selfServerId)
            {
                throw new Exception("Self Declared Dead");
            }

            base.DeclareDead(deadServerId);

            foreach (Partition partition in Partitions.Values)
            {
                if (partition.MasterId == deadServerId)
                {
                    List<string> sortedServerIds = partition.GetSortedServers();
                    int newMasterIndex = sortedServerIds.IndexOf(deadServerId);
                    string newMasterId;
                    do
                    {
                        newMasterIndex = (newMasterIndex + 1) % sortedServerIds.Count;
                        newMasterId = sortedServerIds.ElementAt(newMasterIndex);
                    } while (newMasterId != selfServerId && !GetServer(newMasterId).Alive);

                    ElectNewMaster(partition.Id, newMasterId);
                }
            }

        }

        protected new void ElectNewMaster(string partitionId, string newMasterId)
        {
            // vvv Redundant vvv
            Partition partition = GetPartition(partitionId);
            if (partition.MasterId == selfServerId)
            {
                masterPartitions.Remove(partitionId);
                replicaPartitions.Add(partitionId);
            }
            // ^^^ Redudant ^^^

            base.ElectNewMaster(partitionId, newMasterId);

            if (newMasterId == selfServerId)
            {
                masterPartitions.Add(partitionId);
                replicaPartitions.Remove(partitionId);
            }
            
        }

        public override string ToString()
        {
            string toString = base.ToString();

            toString += "\nPartitions where (self) master:";
            foreach(string partition in masterPartitions)
            {
                toString += " " + partition;
            }

            toString += "\nPartitions where (self) replica:";
            foreach (string partition in replicaPartitions)
            {
                toString += " " + partition;
            }

            return toString;
        }
    }
}
