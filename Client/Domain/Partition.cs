using System;
using System.Collections.Generic;

namespace Client.Domain
{
    class Partition
    {
        private string PartitionId { get; }

        private readonly ISet<string> serverSet;

        public string MasterId { get; }

        public Partition(string partitionId, string masterId, ISet<string> serverSet)
        {
            ValidateParameters(partitionId, masterId, serverSet);

            this.PartitionId = partitionId;
            this.MasterId = masterId;
            this.serverSet = serverSet;
        }

        public bool Contains(string serverId)
        {
            if (String.IsNullOrEmpty(serverId)) return false;
            return serverSet.Contains(serverId);
        }

        public bool IsMaster(string serverId)
        {
            if (String.IsNullOrEmpty(serverId)) return false;
            return serverId.Equals(MasterId);
        }

        public void Print()
        {
            Console.WriteLine("PartitionId: " + PartitionId);
            Console.WriteLine("MasterId: " + MasterId);
            Console.Write("ServerSet: ");
            foreach (string serverId in serverSet)
            {
                Console.Write(serverId + " ");
            }
            Console.Write("\n");
        }

        private void ValidateParameters(string partitionId, string masterId, ISet<string> serverSet)
        {
            if (String.IsNullOrEmpty(partitionId))
            {
                throw new InvalidPartitionCreateArgumentsException("partitionId parameter can't be null or empty.");
            }

            if (String.IsNullOrEmpty(masterId))
            {
                throw new InvalidPartitionCreateArgumentsException("masterId parameter can't be null or empty.");

            }

            if (serverSet == null || serverSet.Count == 0)
            {
                throw new InvalidPartitionCreateArgumentsException("serverSet parameter can't be null or empty.");
            }

            bool masterIsPresent = false;
            foreach (string serverId in serverSet)
            {
                if (String.IsNullOrEmpty(serverId))
                {
                    throw new InvalidPartitionCreateArgumentsException("serverIds within serverSet parameter can't be null or empty.");
                }

                if (serverId.Equals(masterId))
                {
                    masterIsPresent = true;
                }
            }

            if (!masterIsPresent)
            {
                throw new InvalidPartitionCreateArgumentsException("masterId must exist in serverSet parameter.");
            }
        }

    }

    [Serializable]
    public class InvalidPartitionCreateArgumentsException : Exception
    {
        public InvalidPartitionCreateArgumentsException()
        { }

        public InvalidPartitionCreateArgumentsException(string message)
            : base(message)
        { }

        public InvalidPartitionCreateArgumentsException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
