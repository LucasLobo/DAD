using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace GStoreServer.Services
{
    class MasterReplicaServiceImpl : MasterReplicaService.MasterReplicaServiceBase
    {

        private GStore gStore;

        public MasterReplicaServiceImpl(GStore gStore)
        {
            this.gStore = gStore ?? throw new ArgumentNullException("gstore cannot be null");
        }

        public override Task<LockReply> Lock(LockRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteLock(request));
        }

        private LockReply ExecuteLock(LockRequest request)
        {
            Console.WriteLine($"Lock request -> PartitionId: {request.ObjectIdentifier.PartitionId} ObjectId: {request.ObjectIdentifier.ObjectId}");
            GStoreObjectIdentifier gStoreObjectIdentifier = new GStoreObjectIdentifier(request.ObjectIdentifier.PartitionId, request.ObjectIdentifier.ObjectId);
            int lockId = gStore.Lock(gStoreObjectIdentifier);

            return new LockReply
            {
                LockId = lockId
            };
        }

        public override Task<Empty> Write(WriteRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteWrite(request));
        }

        private Empty ExecuteWrite(WriteRequest request)
        {
            Console.WriteLine($"Write Replica request -> PartitionId: {request.Object.ObjectIdentifier.PartitionId} ObjectId: {request.Object.ObjectIdentifier.ObjectId} Value: {request.Object.Value} LockId: {request.LockId}");
            GStoreObjectIdentifier gStoreObjectIdentifier = new GStoreObjectIdentifier(request.Object.ObjectIdentifier.PartitionId, request.Object.ObjectIdentifier.ObjectId);
            gStore.WriteReplica(gStoreObjectIdentifier, request.Object.Value, request.LockId);
            return new Empty();
        }
    }
}
