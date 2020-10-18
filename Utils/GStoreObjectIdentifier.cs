using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace Utils
{
    public class GStoreObjectIdentifier
    {
        public string PartitionId { get; }
        public string ObjectId { get; }

        public GStoreObjectIdentifier(string partition_id, string object_id)
        {
            ValidateParameters(partition_id, object_id);

            PartitionId = partition_id;
            ObjectId = object_id;
        }

        public void ValidateParameters(string partition_id, string object_id)
        {
            if (string.IsNullOrEmpty(partition_id))
            {
                throw new ArgumentException("partition_id parameter can't be null or empty.");
            }

            if (string.IsNullOrEmpty(object_id))
            {
                throw new ArgumentException("object_id parameter can't be null or empty.");
            }

        }

        public override string ToString()
        {
            return "(" + PartitionId + "," + ObjectId + ")";
        }
    }
}
