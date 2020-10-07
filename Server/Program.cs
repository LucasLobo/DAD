using System;
using Grpc.Core;

namespace GStoreServer
{
    class Program
    {
        const int Port = 1001;
        static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { GStoreService.BindService(new ServerService()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("ChatServer server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();

        }
    }
}
