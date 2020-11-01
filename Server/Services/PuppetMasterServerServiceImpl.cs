using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GStoreServer
{
    class PuppetMasterServerServiceImpl : PuppetMasterServerService.PuppetMasterServerServiceBase
    {
        private readonly ManualResetEventSlim freezeLock;
        public PuppetMasterServerServiceImpl(ManualResetEventSlim freezeLock)
        {
            this.freezeLock = freezeLock ?? throw new ArgumentNullException("FreezeLock cannot be null.");
        }

        public override Task<Empty> Crash(Empty request, ServerCallContext context)
        {
            Task.Run(async () =>
            {
                await Task.Delay(150);
                Console.WriteLine("CRASHING...");
                Environment.Exit(1);
            });
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Freeze(Empty request, ServerCallContext context)
        {
            Console.WriteLine("FREEZE");
            freezeLock.Reset();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Status(Empty request, ServerCallContext context)
        {
            Console.WriteLine("STATUS");
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
