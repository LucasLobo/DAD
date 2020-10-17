using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GStoreServer
{
    class ServerService : GStoreService.GStoreServiceBase
    {

        public ServerService() { }

        public override Task<GStoreReadReply> Read(GStoreReadRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteRead(request));
        }

        private GStoreReadReply ExecuteRead(GStoreReadRequest request)
        {
            Console.WriteLine($"Read request -> PartitionId: {request.ObjectIdentifier.PartitionId} ObjectId: {request.ObjectIdentifier.ObjectId}");
            return new GStoreReadReply
            {
                Object = ObjectBuilder.FromObjectIdentifier(request.ObjectIdentifier, "Value")
            };
        }

        public override Task<Empty> Write(GStoreWriteRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteWrite(request));
        }

        private Empty ExecuteWrite(GStoreWriteRequest request)
        {
            Console.WriteLine($"Write request -> PartitionId:{request.Object.ObjectIdentifier.PartitionId} ObjectId: {request.Object.ObjectIdentifier} Value: {request.Object.Value}");
            return new Empty();
        }

        public override Task<GStoreListGlobalReply> ListGlobal(Empty request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteListGlobal());
        }

        private GStoreListGlobalReply ExecuteListGlobal()
        {
            Console.WriteLine("ListGlobal request");
            ObjectIdentifier objId1 = ObjectIdentifierBulder.FromString("partitionId1", "objectId1");
            ObjectIdentifier objId2 = ObjectIdentifierBulder.FromString("partitionId2", "objectId2");
            GStoreListGlobalReply reply = new GStoreListGlobalReply();
            reply.ObjectIdentifiers.Add(objId1);
            reply.ObjectIdentifiers.Add(objId2);
            return reply;
        }

        public override Task<GStoreListServerReply> ListServer(Empty request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteListServer());
        }

        private GStoreListServerReply ExecuteListServer()
        {
            Console.WriteLine($"ListServer request");

            ObjectReplica objectReplica1 = ObjectReplicaBuilder.FromString("partitionId1", "serverId1", "value1", true);
            ObjectReplica objectReplica2 = ObjectReplicaBuilder.FromString("partitionId2", "serverId2", "value2", false);

            GStoreListServerReply reply = new GStoreListServerReply();
            reply.ObjectReplicas.Add(objectReplica1);
            reply.ObjectReplicas.Add(objectReplica2);
            return reply;
        }

        class ObjectIdentifierBulder
        {
            internal static ObjectIdentifier FromString(string partitionId, string objectId)
            {
                return new ObjectIdentifier
                {
                    PartitionId = partitionId,
                    ObjectId = objectId
                };
            }
        }

        class ObjectBuilder
        {

            internal static Object FromString(String partitionId, string objectId, string value)
            {
                return new Object
                {
                    ObjectIdentifier = ObjectIdentifierBulder.FromString(partitionId, objectId),
                    Value = value
                };
            }

            internal static Object FromObjectIdentifier(ObjectIdentifier objectIdentifier, string value)
            {
                return new Object
                {
                    ObjectIdentifier = objectIdentifier,
                    Value = value
                };
            }

        }

        class ObjectReplicaBuilder
        {
            internal static ObjectReplica FromString(string partitionId, string objectId, string value, bool isMasterReplica)
            {
                return new ObjectReplica
                {
                    Object = ObjectBuilder.FromString(partitionId, objectId, value),
                    IsMasterReplica = isMasterReplica
                };
            }
        }
    }
}
