using System;
using System.Collections.Generic;

namespace Client.Domain
{
    class Partition
    {
        private string PartitionId { get; }

        private readonly IDictionary<string, Server> serverSet;

        public Server Master { get; }

        public Partition(string partitionId, Server master, ISet<Server> serverSet)
        {
            ValidateParameters(partitionId, master, serverSet);

            this.PartitionId = partitionId;
            this.Master = master;

            this.serverSet = new Dictionary<string, Server>();
            foreach (Server server in serverSet)
            {
                this.serverSet.Add(server.Id, server);
            }
        }

        public bool Contains(string serverId)
        {
            if (String.IsNullOrEmpty(serverId)) return false;
            return serverSet.ContainsKey(serverId);
        }

        public bool IsMaster(string serverId)
        {
            if (String.IsNullOrEmpty(serverId)) return false;
            return serverId.Equals(Master.Id);
        }

        public void Print()
        {
            Console.WriteLine("PartitionId: " + PartitionId);
            Console.WriteLine("MasterId: " + Master.Id);
            Console.Write("ServerSet: ");

            foreach (KeyValuePair<string, Server> entry in serverSet)
            {
                Console.Write(entry.Key + " ");
            }
            Console.Write("\n");
        }

        private void ValidateParameters(string partitionId, Server master, ISet<Server> serverSet)
        {
            if (String.IsNullOrEmpty(partitionId))
            {
                throw new ArgumentException("partitionId parameter can't be null or empty.");
            }

            if (master == null)
            {
                throw new ArgumentException("masterId parameter can't be null or empty.");
            }

            if (serverSet == null || serverSet.Count == 0)
            {
                throw new ArgumentException("serverSet parameter can't be null or empty.");
            }

            if (!serverSet.Contains(master))
            {
                throw new ArgumentException("masterId must exist in serverSet parameter.");
            }
        }

    }
}
