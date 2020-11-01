using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Threading.Tasks;
using Utils;

namespace GStoreServer.Services
{
    class ServerServiceImpl : GStoreService.GStoreServiceBase
    {
        private GStore gStore;
        public ServerServiceImpl(GStore gStore)
        {
            this.gStore = gStore;
        }

        public override Task<GStoreReadReply> Read(GStoreReadRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteRead(request));
        }

        private GStoreReadReply ExecuteRead(GStoreReadRequest request)
        {
            Console.WriteLine($"Read request -> PartitionId: {request.ObjectIdentifier.PartitionId} ObjectId: {request.ObjectIdentifier.ObjectId}");
            GStoreObjectIdentifier gStoreObjectIdentifier = new GStoreObjectIdentifier(request.ObjectIdentifier.PartitionId, request.ObjectIdentifier.ObjectId);
            string value = gStore.Read(gStoreObjectIdentifier);

            if (value == null) value = "N/A";
            return new GStoreReadReply
            {
                Object = DataObjectBuilder.FromObjectIdentifier(request.ObjectIdentifier, value)
            };
        }

        public override Task<Empty> Write(GStoreWriteRequest request, ServerCallContext context)
        {
            return ExecuteWrite(request);
        }

        private async Task<Empty> ExecuteWrite(GStoreWriteRequest request)
        {
            Console.WriteLine($"Write request -> PartitionId: {request.Object.ObjectIdentifier.PartitionId} ObjectId: {request.Object.ObjectIdentifier} Value: {request.Object.Value}");

            GStoreObjectIdentifier gStoreObjectIdentifier = new GStoreObjectIdentifier(request.Object.ObjectIdentifier.PartitionId, request.Object.ObjectIdentifier.ObjectId);
            await gStore.Write(gStoreObjectIdentifier, request.Object.Value);

            return new Empty();
        }

        public override Task<GStoreListGlobalReply> ListGlobal(Empty request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteListGlobal());
        }

        private GStoreListGlobalReply ExecuteListGlobal()
        {
            Console.WriteLine("ListGlobal request");
            DataObjectIdentifier objId1 = DataObjectIdentifierBuilder.FromString("partitionId1", "objectId1");
            DataObjectIdentifier objId21 = DataObjectIdentifierBuilder.FromString("partitionId2", "objectId2");
            DataObjectIdentifier objId22 = DataObjectIdentifierBuilder.FromString("partitionId2", "objectId2");
            DataObjectIdentifier objId3 = DataObjectIdentifierBuilder.FromString("partitionId3", "objectId3");
            GStoreListGlobalReply reply = new GStoreListGlobalReply();
            reply.ObjectIdentifiers.Add(objId1);
            reply.ObjectIdentifiers.Add(objId21);
            reply.ObjectIdentifiers.Add(objId22);
            reply.ObjectIdentifiers.Add(objId3);
            return reply;
        }

        public override Task<GStoreListServerReply> ListServer(Empty request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteListServer());
        }

        private GStoreListServerReply ExecuteListServer()
        {
            Console.WriteLine($"ListServer request");

            DataObjectReplica objectReplica1 = DataObjectReplicaBuilder.FromString("partitionId1", "serverId1", "value1", true);
            DataObjectReplica objectReplica21 = DataObjectReplicaBuilder.FromString("partitionId2", "serverId2", "value2", false);
            DataObjectReplica objectReplica22 = DataObjectReplicaBuilder.FromString("partitionId2", "serverId2", "value2", true);
            DataObjectReplica objectReplica23 = DataObjectReplicaBuilder.FromString("partitionId2", "serverId2", "value3", true);

            GStoreListServerReply reply = new GStoreListServerReply();
            reply.ObjectReplicas.Add(objectReplica1);
            reply.ObjectReplicas.Add(objectReplica21);
            reply.ObjectReplicas.Add(objectReplica22);
            reply.ObjectReplicas.Add(objectReplica23);
            return reply;
        }

        class DataObjectIdentifierBuilder
        {
            internal static DataObjectIdentifier FromString(string partitionId, string objectId)
            {
                return new DataObjectIdentifier
                {
                    PartitionId = partitionId,
                    ObjectId = objectId
                };
            }
        }

        class DataObjectBuilder
        {

            internal static DataObject FromString(String partitionId, string objectId, string value)
            {
                return new DataObject
                {
                    ObjectIdentifier = DataObjectIdentifierBuilder.FromString(partitionId, objectId),
                    Value = value
                };
            }

            internal static DataObject FromObjectIdentifier(DataObjectIdentifier objectIdentifier, string value)
            {
                return new DataObject
                {
                    ObjectIdentifier = objectIdentifier,
                    Value = value
                };
            }

        }

        class DataObjectReplicaBuilder
        {
            internal static DataObjectReplica FromString(string partitionId, string objectId, string value, bool isMasterReplica)
            {
                return new DataObjectReplica
                {
                    Object = DataObjectBuilder.FromString(partitionId, objectId, value),
                    IsMasterReplica = isMasterReplica
                };
            }
        }
    }
}
