using System;
using System.Collections.Generic;

namespace Utils
{
    public class Partition : IEquatable<Partition>
    {
        public string Id { get; }

        public string MasterId { get; set; }

        public ISet<string> ReplicaSet { get; }

        public Partition(string id, string masterId, ISet<string> replicaSet)
        {
            Id = id;
            MasterId = masterId;
            ReplicaSet = replicaSet;
        }

        public bool Contains(string serverId)
        {
            if (string.IsNullOrEmpty(serverId)) return false;
            return (MasterId == serverId) || ReplicaSet.Contains(serverId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Partition);
        }

        public bool Equals(Partition other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public bool Equals(string otherId)
        {
            return otherId != null &&
                   Id == otherId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            string partition = $"Partition: {Id}, Master: {MasterId}, Replicas: ";

            foreach(string replicaId in ReplicaSet)
            {
                partition += $"{replicaId} ";
            }

            return partition;
        }
    }
}
