using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;

namespace GStoreServer
{
    class PuppetMasterServerService : PuppetMasterServerServices.PuppetMasterServerServicesBase
    {

        public override Task<Empty> Freeze(Empty request, ServerCallContext context)
        {
            Console.WriteLine("FREEZE");
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Unfreeze(Empty request, ServerCallContext context)
        {
            Console.WriteLine("UNFREEZE");

            return Task.FromResult(new Empty());
        }
    }
}
