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

        public ServerService () { }

        public override Task<GStoreReadReply> Read(GStoreReadRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteRead(request));
        }

        private GStoreReadReply ExecuteRead(GStoreReadRequest request)
        {
            Console.WriteLine($"Read request-> PartitionId: {request.PartitionId} ObjectId:{request.ObjectId}");
            return new GStoreReadReply
            {
                ObjectValue = "Test"
            };
        }

        public override Task<Empty> Write(GStoreWriteRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteWrite(request));
        }

        private Empty ExecuteWrite(GStoreWriteRequest request)
        {
            Console.WriteLine($"Write request-> PartitionId:{request.PartitionId} ObjectId: {request.ObjectId} Value: {request.ObjectValue}");
            return new Empty();
        }

        public override Task<GStoreListGlobalReply> ListGlobal(Empty request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteListGlobal());
        }

        private GStoreListGlobalReply ExecuteListGlobal()
        {
            ObjectIdentifier objId1 = new ObjectIdentifier { PartitionId = "partitionId1", ObjectId = "objectId1" };
            ObjectIdentifier objId2 = new ObjectIdentifier { PartitionId = "partitionId2", ObjectId = "objectId2" };
            Console.WriteLine("ListGlobal request");
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
            ObjectIdentifier objId1 = new ObjectIdentifier { PartitionId = "partitionId1", ObjectId = "objectId1" };
            ObjectIdentifier objId2 = new ObjectIdentifier { PartitionId = "partitionId2", ObjectId = "objectId2" };
            Object obj1 = new Object { IsMasterReplica = true, ObjectIdentifier = objId1, Value = "value1" };
            Object obj2 = new Object { IsMasterReplica = true, ObjectIdentifier = objId2, Value = "value2" };

            Console.WriteLine($"ListServer request");

            GStoreListServerReply reply = new GStoreListServerReply();
            reply.Objects.Add(obj1);
            reply.Objects.Add(obj2);
            return reply;
        }
    }
}
