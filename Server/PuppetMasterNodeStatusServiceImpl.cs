using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PuppetMasterNodeStatusServiceImpl : PuppetMasterNodeStatusService.PuppetMasterNodeStatusServiceBase
    {
        public override Task<Empty> Status(Empty request, ServerCallContext context)
        {
            Console.WriteLine("STATUS");
            //TODO: Later add more stuff to print
            return Task.FromResult(new Empty());
        }
    }
}
