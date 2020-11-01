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
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(masterId))
            {
                throw new ArgumentException($"'{nameof(masterId)}' cannot be null or whitespace", nameof(masterId));
            }

            Id = id;
            MasterId = masterId;
            ReplicaSet = replicaSet ?? throw new ArgumentNullException(nameof(replicaSet));
        }

        public bool Contains(string serverId)
        {
            if (string.IsNullOrEmpty(serverId)) return false;
            return (MasterId == serverId) || ReplicaSet.Contains(serverId);
        }

        public void DeclareDead(string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentException($"'{nameof(serverId)}' cannot be null or whitespace");
            }

            // todo
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
