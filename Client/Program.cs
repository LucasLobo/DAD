using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Commands;
using Client.Domain;
using Google.Protobuf.WellKnownTypes;
using Utils;
using Grpc.Core;
using System.Diagnostics;
using Grpc.Net.Client;

namespace Client
{

    class Program
    {

        private static void RegisterCommands(CommandDispatcher commandDispatcher, ConnectionManager connectionManager)
        {
            commandDispatcher.Register("read", new ReadCommand(connectionManager));
            commandDispatcher.Register("write", new WriteCommand(connectionManager));
            commandDispatcher.Register("listServer", new ListServerCommand(connectionManager));
            commandDispatcher.Register("listGlobal", new ListGlobalCommand(connectionManager));
            commandDispatcher.Register("wait", new WaitCommand());
        }

        public static ConnectionManager CreateConnectionManager()
        {
            ConnectionManager connectionManager = CreateClientConnectionManager();
            Console.WriteLine(connectionManager);
            Console.WriteLine();
            return connectionManager;
        }

        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: Expected a script name but received none.");
                return;
            }

            else if (args.Length > 1)
            {
                Console.WriteLine("WARNING: Expected 1 argument but received " + (args.Length - 1) + ".");
                Console.WriteLine();
            }

            String filename = args[0];

            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(filename);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("ERROR: File " + filename + " not found in current directory.");
                Console.WriteLine(e);
                return;
            }

            CommandDispatcher commandDispatcher = new CommandDispatcher();
            ConnectionManager connectionManager = CreateConnectionManager();
            RegisterCommands(commandDispatcher, connectionManager);

            try
            {
                int Port = 8085;
                Grpc.Core.Server server = new Grpc.Core.Server
                {
                    Services =
                    {
                        PuppetMasterClientService.BindService(new PuppetmasterClientServiceImpl())
                    },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };
                Console.WriteLine("Client listening on port " + Port);
                

                server.Start();

                List<string> preprocessed = CommandPreprocessor.Preprocess(lines);

                var timer = new Stopwatch();
                timer.Start();
                Task dispatcher = commandDispatcher.ExecuteAllAsync(preprocessed.ToArray());

                //for (int i = 0; i < 15; i++)
                //{
                //    Console.WriteLine("---");
                //    await Task.Delay(500);
                //}

                await dispatcher;
                timer.Stop();
                Console.WriteLine(timer.ElapsedMilliseconds);

                Console.WriteLine("Press any key to stop the client...");
                Console.ReadKey();
                Console.WriteLine("\nShutting down...");
                server.ShutdownAsync().Wait();
            }
            catch (PreprocessingException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        private static ConnectionManager CreateClientConnectionManager()
        {
            int serverNumber = 5;
            int partitionSize = 3;

            IDictionary<string, Domain.Server> servers = new Dictionary<string, Domain.Server>();
            IDictionary<string, Partition> partitions = new Dictionary<string, Partition>();
            for (int i = 1; i <= serverNumber; i++)
            {
                string serverId = "s-" + i;
                int port = 8080 + i;
                string address = "http://localhost:" + port;
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                GStoreService.GStoreServiceClient stub = new GStoreService.GStoreServiceClient(channel);
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
                partitions.Add(partitionId, partition);
            }

            return new ConnectionManager(servers, partitions);
        }
    }
}
