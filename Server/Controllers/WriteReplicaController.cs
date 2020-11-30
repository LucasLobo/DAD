using GStoreServer.Domain;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Utils;

namespace GStoreServer.Controllers
{
    class WriteReplicaController
    {
        private static async Task ExecuteReplicaAsync(ConnectionManager connectionManager, MasterReplicaService.MasterReplicaServiceClient stub, GStoreObject gStoreObject, int writeRequestId)
        {
            WriteRequest writeRequest = new WriteRequest
            {
                Object = new ObjectDto
                {
                    ObjectIdentifier = new ObjectIdentifierDto
                    {
                        PartitionId = gStoreObject.Identifier.PartitionId,
                        ObjectId = gStoreObject.Identifier.ObjectId
                    },
                    Value = gStoreObject.Value
                },
                WriteRequestId = writeRequestId
            };

            await stub.WriteAsync(writeRequest);
        }

        public static async Task ExecuteAsync(ConnectionManager connectionManager, GStoreObject gStoreObject, int writeRequestId)
        {

            GStoreObjectIdentifier gStoreObjectIdentifier = gStoreObject.Identifier;

            // Get all replicas associated to this Partition
            IImmutableSet<Server> replicas = connectionManager.GetPartitionAliveReplicas(gStoreObjectIdentifier.PartitionId);

            IDictionary<string, Task> writeTasks = new Dictionary<string, Task>();
            foreach (Server replica in replicas)
            {
                writeTasks.Add(replica.Id, ExecuteReplicaAsync(connectionManager, replica.Stub, gStoreObject, writeRequestId));
            }

            foreach (KeyValuePair<string, Task> writeTaskPair in writeTasks)
            {
                string replicaId = writeTaskPair.Key;
                try
                {
                    await writeTaskPair.Value;
                }
                catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.Internal)
                {
                    connectionManager.DeclareDead(replicaId);
                }
            }
        }
    }
}
