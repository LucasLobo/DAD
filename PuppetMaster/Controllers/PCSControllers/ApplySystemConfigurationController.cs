using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PuppetMaster.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Controllers.PCSControllers
{
    class ApplySystemConfigurationController
    {
        public static async Task Execute(ConnectionManager connectionManager, List<string> configurationLines)
        {
            List<AsyncUnaryCall<Empty>> asyncUnaryCalls = new List<AsyncUnaryCall<Empty>>();
            foreach (string line in configurationLines)
            {
                //TODO line parser and then Add
            }

            List<Empty> statusReplies = new List<Empty>();
            foreach (AsyncUnaryCall<Empty> request in asyncUnaryCalls)
            {
                statusReplies.Add(await request.ResponseAsync);
            }
        }
    }
}
