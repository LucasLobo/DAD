using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using GStoreServer.Domain;
using GStoreServer.Services;
using Utils;

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

            ConnectionManager connectionManager = CreateServerConnectionManager();
            Console.WriteLine(connectionManager);

            GStore gStore = new GStore(connectionManager);
            RequestInterceptor requestInterceptor = new RequestInterceptor(freezeLock, minDelay, maxDelay);

            Grpc.Core.Server server = new Grpc.Core.Server
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
                server = new Grpc.Core.Server
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

        private static ConnectionManager CreateServerConnectionManager()
        {
            int serverNumber = 5;
            int partitionSize = 3;

            IDictionary<string, Domain.Server> servers = new Dictionary<string, Domain.Server>();
            IDictionary<string, Partition> partitions = new Dictionary<string, Partition>();
            ISet<string> masterPartitions = new HashSet<string>();
            ISet<string> replicaPartitions = new HashSet<string>();

            for (int i = 1; i <= serverNumber; i++)
            {
                string serverId = "s-" + i;
                int port = 8080 + i;
                string address = "http://localhost:" + port;
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                MasterReplicaService.MasterReplicaServiceClient stub = new MasterReplicaService.MasterReplicaServiceClient(channel);
                Domain.Server server = new Domain.Server(serverId, stub);
                servers.Add(serverId, server);
            }

            for (int i = 1; i <= serverNumber; i++)
            {
                string masterId = "s-" + i;
                ISet<string> partitionServerSet = new HashSet<string>();

                for (int j = 1; j < partitionSize; j++)
                {
                    string serverId = "s-" + ((i + j - 1) % serverNumber + 1);
                    partitionServerSet.Add(serverId);
                }

                string partitionId = "part-" + i;
                Partition partition = new Partition(partitionId, masterId, partitionServerSet);

                if (i == 1) masterPartitions.Add(partitionId);
                else replicaPartitions.Add(partitionId);

                partitions.Add(partitionId, partition);
            }
            ConnectionManager connectionManager = new ConnectionManager(servers, partitions, masterPartitions, replicaPartitions, "s-1");
            connectionManager.DeclareDead("s-3");
            connectionManager.DeclareDead("s-4");
            connectionManager.DeclareDead("s-5");
            return connectionManager;
        }
    }
}
