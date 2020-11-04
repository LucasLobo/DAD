using Client.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Client.Controllers
{
    class ListServerController
    {

        public static async Task<HashSet<GStoreObjectReplica>> Execute(ConnectionManager connectionManager, string serverId)
        {
            Server server;
            server = connectionManager.GetAliveServer(serverId);
            GStoreListServerReply gStoreListServerReply = await server.Stub.ListServerAsync(new Google.Protobuf.WellKnownTypes.Empty());

            HashSet<GStoreObjectReplica> gStoreObjectReplicas = new HashSet<GStoreObjectReplica>();
            foreach (DataObjectReplica dataObjectReplica in gStoreListServerReply.ObjectReplicas)
            {
                gStoreObjectReplicas.Add(CreateObjectReplica(dataObjectReplica));
            }
            return gStoreObjectReplicas;
        }

        private static GStoreObjectReplica CreateObjectReplica(DataObjectReplica dataObjectReplica)
        {
            GStoreObjectIdentifier gStoreObjectIdentifier = new GStoreObjectIdentifier(dataObjectReplica.Object.ObjectIdentifier.PartitionId, dataObjectReplica.Object.ObjectIdentifier.ObjectId);
            GStoreObject gStoreObject = new GStoreObject(gStoreObjectIdentifier, dataObjectReplica.Object.Value);
            return new GStoreObjectReplica(gStoreObject, dataObjectReplica.IsMasterReplica);
        }
    }
}
