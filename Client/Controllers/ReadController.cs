using Client.Domain;
using System;
using System.Threading.Tasks;
using Utils;

namespace Client.Controllers
{
    class ReadController
    {
        public static async Task<GStoreObject> Execute(ConnectionManager connectionManager, string partitionId, string serverId, string objectId)
        {
            Server server = connectionManager.ChooseServerForRead(partitionId, serverId);
            Console.WriteLine($"Trying: {server.Id}");
            GStoreReadRequest gStoreReadRequest = new GStoreReadRequest()
            {
                ObjectIdentifier = new DataObjectIdentifier
                {
                    PartitionId = partitionId,
                    ObjectId = objectId
                }
            };

            GStoreReadReply gStoreReadReply = await server.Stub.ReadAsync(gStoreReadRequest);

            return CreateObject(gStoreReadReply.Object);
        }

        private static GStoreObject CreateObject(DataObject gStoreObject)
        {
            GStoreObjectIdentifier gStoreObjectIdentifier = new GStoreObjectIdentifier(gStoreObject.ObjectIdentifier.PartitionId, gStoreObject.ObjectIdentifier.ObjectId);
            return new GStoreObject(gStoreObjectIdentifier, gStoreObject.Value);
        }
    }
}
