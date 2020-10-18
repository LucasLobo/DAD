using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Utils
{
    public class GStoreObjectIdentifier : IEquatable<GStoreObjectIdentifier>
    {
        public readonly string PartitionId;
        public readonly string ObjectId;

        public GStoreObjectIdentifier(string partitionId, string objectId)
        {
            ValidateParameters(partitionId, objectId);

            PartitionId = partitionId;
            ObjectId = objectId;
        }

        public void ValidateParameters(string partitionId, string objectId)
        {
            if (string.IsNullOrEmpty(partitionId))
            {
                throw new ArgumentException("partition_id parameter can't be null or empty.");
            }

            if (string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentException("object_id parameter can't be null or empty.");
            }
        }

        public override string ToString()
        {
            return "(" + PartitionId + "," + ObjectId + ")";
        }

        public bool Equals([AllowNull] GStoreObjectIdentifier other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return PartitionId.Equals(other.PartitionId) && ObjectId.Equals(other.ObjectId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PartitionId, ObjectId);
        }
    }
}
