using System;
using System.Collections.Generic;
using System.Linq;
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
        private static string myServerId;

        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            Console.WriteLine(args[0] + " : " + args[1] + " : " + args[2] + " : " + args[3] + " : " + args[4] + " : ");
            
            myServerId = args[0];
            string[] protocolAndHostnameAndPort = args[1].Split("://");
            string[] hotnameAndPort = protocolAndHostnameAndPort[1].Split(":");
            int port = Int32.Parse(hotnameAndPort[1]);
            int minDelay = Int32.Parse(args[2]);
            int maxDelay = Int32.Parse(args[3]);
            ManualResetEventSlim freezeLock = new ManualResetEventSlim(true);

            ConnectionManager connectionManager = CreateServerConnectionManager(args[4]);
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
                Ports = { new ServerPort(hotnameAndPort[0], port, ServerCredentials.Insecure) }
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

            server.Start();
            Console.WriteLine("GStore server listening on port " + port);
            Console.WriteLine("Press any key to stop the server...");

            /*try
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
            }*/
            Console.ReadKey();
            Console.WriteLine("\nShutting down...");
            server.ShutdownAsync().Wait();
        }

        private static ConnectionManager CreateServerConnectionManager(string networkConfiguration)
        {
            InitializationParser initializationParser = new InitializationParser(networkConfiguration);
            List<Tuple<string, string>> serversConfiguration = initializationParser.getServersConfiguration();
            List<Tuple<string, List<string>>> partitionsConfiguration = initializationParser.getPartitionsConfiguration();

            IDictionary<string, Domain.Server> servers = new Dictionary<string, Domain.Server>();
            IDictionary<string, Partition> partitions = new Dictionary<string, Partition>();
            ISet<string> masterPartitions = new HashSet<string>();
            ISet<string> replicaPartitions = new HashSet<string>();

            for (int i = 1; i < serversConfiguration.Count; i++)
            {
                Tuple<string, string> serverConfig = serversConfiguration[i];
                string serverId = serverConfig.Item1;
                if (myServerId != serverId)
                {
                    string address = serverConfig.Item2;
                    GrpcChannel channel = GrpcChannel.ForAddress(address);
                    MasterReplicaService.MasterReplicaServiceClient stub = new MasterReplicaService.MasterReplicaServiceClient(channel);
                    Domain.Server server = new Domain.Server(serverId, stub);
                    servers.Add(serverId, server);
                }
            }

            foreach (Tuple<string, List<string>> partitionConfig in partitionsConfiguration)
            {
                string partitionId = partitionConfig.Item1;
                string masterId = partitionConfig.Item2.ElementAt(0);
                ISet<string> partitionServerSet = new HashSet<string>();

                foreach (string serverId in partitionConfig.Item2)
                {
                    partitionServerSet.Add(serverId);
                }

                Partition partition = new Partition(partitionId, masterId, partitionServerSet);

                if (masterId == myServerId) masterPartitions.Add(partitionId);
                else replicaPartitions.Add(partitionId);

                partitions.Add(partitionId, partition);
            }
            ConnectionManager connectionManager = new ConnectionManager(servers, partitions, masterPartitions, replicaPartitions, myServerId);
            return connectionManager;
        }
    }
}
