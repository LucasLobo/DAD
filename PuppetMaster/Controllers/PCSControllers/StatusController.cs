using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PuppetMaster.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Controllers.PCSControllers
{
    class StatusController
    {
        public static async Task Execute(ConnectionManager connectionManager)
        {
            List<AsyncUnaryCall<Empty>> asyncUnaryCalls = new List<AsyncUnaryCall<Empty>>();
            foreach (Domain.Server server in connectionManager.GetAllServerStubs())
            {
                asyncUnaryCalls.Add(server.Stub.StatusAsync(new Empty()));
            }

            foreach (Client client in connectionManager.GetAllClientStubs())
            {
                asyncUnaryCalls.Add(client.Stub.StatusAsync(new Empty()));
            }

            // this is a dummy implementation (in the future it will receive information)
            List<Empty> statusReplies = new List<Empty>();
            foreach (AsyncUnaryCall<Empty> request in asyncUnaryCalls)
            {
                statusReplies.Add(await request.ResponseAsync);
            }
        }
    }
}
