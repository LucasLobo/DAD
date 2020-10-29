using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Utils;

namespace GStoreServer
{
    class Program
    {
        const int Port = 8081;
        static void Main(string[] args)
        {
            int minDelay = 0;
            int maxDelay = 0;
            ManualResetEventSlim freezeLock = new ManualResetEventSlim(true);
            GStore gStore = new GStore();

            Server server = new Server
            {
                Services =
                {
                    GStoreService.BindService(new ServerServiceImpl(gStore)).Intercept(new RequestInterceptor(freezeLock, minDelay, maxDelay)),
                    PuppetMasterServerService.BindService(new PuppetMasterServerServiceImpl(freezeLock))
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };

            //GStoreObject obj1 = new GStoreObject( "v1");
            //GStoreObject obj2 = new GStoreObject(new GStoreObjectIdentifier("1", "2"), "v2");
            //GStoreObject obj3 = new GStoreObject(new GStoreObjectIdentifier("1", "3"), "v3");
            //Console.WriteLine(gstoreserver.AddObject(obj1));
            //Console.WriteLine(gstoreserver.AddObject(obj2));
            //Console.WriteLine(gstoreserver.AddObject(obj3));
            //Console.WriteLine(gstoreserver.AddObject(obj3));

            //gstoreserver.ShowDataStore();
            //gstoreserver.UpdateObject(new GStoreObject(new GStoreObjectIdentifier("1", "2"), "novo"));
            //Console.WriteLine("\n");
            //gstoreserver.ShowDataStore();

            //Console.WriteLine(gstoreserver.Read(new GStoreObjectIdentifier("1", "4")));

            Console.WriteLine("GStore server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            server.Start();
            Console.ReadKey();
            Console.WriteLine("\nShutting down...");
            server.ShutdownAsync().Wait();
        }
    }
}
