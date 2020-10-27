using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PCS
{
    class PCSService : PuppetMasterPCSService.PuppetMasterPCSServiceBase
    {
        private const string CLIENT_LOCATION = "../../../../Client/bin/Debug/netcoreapp3.1/Client.exe";
        private const string SERVER_LOCATION = "../../../../Server/bin/Debug/netcoreapp3.1/Server.exe";

        public PCSService() { }

        public override Task<Empty> Client(ClientRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteClient(request));
        }

        private Empty ExecuteClient(ClientRequest request)
        {
            Console.WriteLine($"Create Client request-> Username: {request.Username} Client_URL: {request.ClientUrl} Script: {request.ScriptFile}");

            ProcessStartInfo clientInfo = new ProcessStartInfo
            {
                FileName = CLIENT_LOCATION,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                //Arguments = $"{request.Username} {request.ClientUrl} {request.ScriptFile}"
            };
            Process exeClientProcess = Process.Start(clientInfo);

            Console.WriteLine($"Create Client DONE");
            return new Empty();
        }

        public override Task<Empty> Server(ServerRequest request, ServerCallContext context)
        {
            return Task.FromResult(ExecuteServer(request));
        }

        private Empty ExecuteServer(ServerRequest request)
        {
            Console.WriteLine($"Create Server request-> Server_ID: {request.ServerId} URL: {request.Url} Min-Delay: {request.MinDelay} Max-Delay: {request.MaxDelay}");

            ProcessStartInfo serverInfo = new ProcessStartInfo
            {
                FileName = SERVER_LOCATION,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                //Arguments = $"{request.ServerId} {request.Url} {request.MinDelay} {request.MaxDelay}"
            };
            Process exeServerProcess = Process.Start(serverInfo);

            Console.WriteLine($"Create Server DONE");
            return new Empty();
        }
    }
}
