using System.Threading.Tasks;
using Utils;

namespace GStoreServer.Controllers
{
    class LockController
    {
        public static async Task<int> Execute(MasterReplicaService.MasterReplicaServiceClient Stub, GStoreObjectIdentifier gStoreObjectIdentifier)
        {
            LockRequest lockRequest = new LockRequest
            {
                ObjectIdentifier = new ObjectIdentifierDto
                {
                    PartitionId = gStoreObjectIdentifier.PartitionId,
                    ObjectId = gStoreObjectIdentifier.ObjectId
                }
            };

            LockReply lockReply = await Stub.LockAsync(lockRequest);
            return lockReply.LockId;
        }
    }
}
