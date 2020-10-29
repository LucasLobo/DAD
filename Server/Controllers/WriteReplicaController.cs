using System.Threading.Tasks;
using Utils;

namespace GStoreServer.Controllers
{
    class WriteReplicaController
    {
        public static async Task Execute(MasterReplicaService.MasterReplicaServiceClient Stub, GStoreObject gStoreObject, int lockId)
        {
            WriteRequest writeRequest = new WriteRequest
            {
                LockId = lockId,
                Object = new ObjectDto
                {
                    ObjectIdentifier = new ObjectIdentifierDto
                    {
                        PartitionId = gStoreObject.Identifier.PartitionId,
                        ObjectId = gStoreObject.Identifier.ObjectId
                    },
                    Value = gStoreObject.Value
                }
            };

            await Stub.WriteAsync(writeRequest);
        }
    }
}
