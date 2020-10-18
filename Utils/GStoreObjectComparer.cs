using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Utils
{
    public class GStoreObjectComparer : IEqualityComparer<GStoreObjectIdentifier>
    {
        public bool Equals([AllowNull] GStoreObjectIdentifier x, [AllowNull] GStoreObjectIdentifier y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("identifiers can't be null");
            }

            if (x.ObjectId.Equals(y.ObjectId) && x.PartitionId.Equals(y.PartitionId))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode([DisallowNull] GStoreObjectIdentifier obj)
        {
            return 1;
        }
    }
}
