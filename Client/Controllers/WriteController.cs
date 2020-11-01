using Client.Domain;
using System;
using System.Threading.Tasks;

namespace Client.Controllers
{
    class WriteController
    {
        public static async Task Execute(ConnectionManager connectionManager, string partitionId, string objectId, string value)
        {
            Server server = connectionManager.ChooseServerForWrite(partitionId);
            Console.WriteLine($"Trying: {server.Id}");
            GStoreWriteRequest writeRequest = new GStoreWriteRequest()
            {
                Object = new DataObject()
                {
                    ObjectIdentifier = new DataObjectIdentifier
                    {
                        PartitionId = partitionId,
                        ObjectId = objectId
                    },
                    Value = value
                }
            };

            await server.Stub.WriteAsync(writeRequest);
        }
    }
}
