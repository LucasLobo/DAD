using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GStoreServer
{
    class PuppetMasterServerService : PuppetMasterServerServices.PuppetMasterServerServicesBase
    {
        private readonly ManualResetEventSlim freezeLock;
        public PuppetMasterServerService(ManualResetEventSlim freezeLock)
        {
            this.freezeLock = freezeLock ?? throw new ArgumentNullException("ReaderWriter lock cannot be null.");
        }

        public override Task<Empty> Freeze(Empty request, ServerCallContext context)
        {
            Console.WriteLine("FREEZE");
            freezeLock.Reset();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Unfreeze(Empty request, ServerCallContext context)
        {
            Console.WriteLine("UNFREEZE");
            freezeLock.Set();
            return Task.FromResult(new Empty());
        }
    }
}
