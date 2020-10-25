using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class PuppetMasterNodeStatusServiceImpl : PuppetMasterNodeStatusService.PuppetMasterNodeStatusServiceBase
    {
        public override Task<Empty> Status(Empty request, ServerCallContext context)
        {
            Console.WriteLine("STATUS");
            return Task.FromResult(new Empty());
        }
    }
}
