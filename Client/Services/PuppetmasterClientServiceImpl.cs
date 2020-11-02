using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Client
{
    class PuppetmasterClientServiceImpl : PuppetMasterClientService.PuppetMasterClientServiceBase
    {
        public override Task<Empty> Status(Empty request, ServerCallContext context)
        {
            Console.WriteLine("STATUS");
            return Task.FromResult(new Empty());
        }
    }
}
