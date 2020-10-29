using System;
using System.Threading;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using GStoreServer.Services;

namespace GStoreServer
{
    class Program
    {
        const int Port = 8081;
        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            int minDelay = 0;
            int maxDelay = 0;
            ManualResetEventSlim freezeLock = new ManualResetEventSlim(true);

            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:8082");
            MasterReplicaService.MasterReplicaServiceClient Stub = new MasterReplicaService.MasterReplicaServiceClient(channel);

            GStore gStore = new GStore(Stub);
            RequestInterceptor requestInterceptor = new RequestInterceptor(freezeLock, minDelay, maxDelay);

            Server server = new Server
            {
                Services =
                {
                    GStoreService.BindService(new ServerServiceImpl(gStore)).Intercept(requestInterceptor),
                    MasterReplicaService.BindService(new MasterReplicaServiceImpl(gStore)).Intercept(requestInterceptor),
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

            

            try
            {
                server.Start();
                Console.WriteLine("GStore server listening on port " + Port);
                Console.WriteLine("Press any key to stop the server...");

            } catch (Exception)
            {
                server = new Server
                {
                    Services =
                {
                    GStoreService.BindService(new ServerServiceImpl(gStore)).Intercept(requestInterceptor),
                    MasterReplicaService.BindService(new MasterReplicaServiceImpl(gStore)).Intercept(requestInterceptor),
                    PuppetMasterServerService.BindService(new PuppetMasterServerServiceImpl(freezeLock))
                },
                    Ports = { new ServerPort("localhost", Port+1, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("GStore server listening on port " + (Port+1));
                Console.WriteLine("Press any key to stop the server...");
            }
            Console.ReadKey();
            Console.WriteLine("\nShutting down...");
            server.ShutdownAsync().Wait();
        }
    }
}
