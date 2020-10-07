using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
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
            Console.WriteLine($"Read request-> PartitionId: {request.PartitionId} ObjectId:{request.ObjectId} ServerId: {request.ServerId}");
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
            return Task.FromResult(ExecuteListGlobal(request));
        }

        private GStoreListGlobalReply ExecuteListGlobal(Empty request)
        {
            Console.WriteLine("ListGlobal request");
            return new GStoreListGlobalReply
            {
                ObjectIdentifiers = "Identifiers"
            };
        }

        public override Task<GStoreListServerReply> ListServer(GStoreListServerRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteListServer(request));
        }

        private GStoreListServerReply ExecuteListServer(GStoreListServerRequest request)
        {
            Console.WriteLine($"ListServer request-> ServerId:{request.ServerId}");
            return new GStoreListServerReply
            {
                IsMasterReplica = true,
                Objects = "Objects"
            };
        }
    }
}
