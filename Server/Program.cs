using System;
using Grpc.Core;
using Utils;

namespace GStoreServer
{
    class Program
    {
        const int Port = 8081;
        static void Main(string[] args)
        {
            /*Server server = new Server
            {
                Services = { GStoreService.BindService(new ServerService()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("GStore server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();*/
            GStoreObject obj1 = new GStoreObject(new GStoreObjectIdentifier("1", "1"), "v1");
            GStoreObject obj2 = new GStoreObject(new GStoreObjectIdentifier("1", "2"), "v2");
            GStoreObject obj3 = new GStoreObject(new GStoreObjectIdentifier("1", "3"), "v3");
            GStoreServer server = new GStoreServer();
            Console.WriteLine(server.AddObject(obj1));
            Console.WriteLine(server.AddObject(obj2));
            Console.WriteLine(server.AddObject(obj3));
            Console.WriteLine(server.AddObject(obj3));

            server.ShowDataStore();
            server.UpdateObject(new GStoreObject(new GStoreObjectIdentifier("1", "2"), "novo"));
            Console.WriteLine("\n");
            server.ShowDataStore();

            Console.WriteLine(server.ReadValue(new GStoreObjectIdentifier("1", "4")));

        }
    }
}
