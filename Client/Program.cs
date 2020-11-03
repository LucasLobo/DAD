using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Commands;
using Client.Domain;
using Utils;
using Grpc.Core;
using System.Diagnostics;
using Grpc.Net.Client;
using System.Linq;

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

        public static ConnectionManager CreateConnectionManager(string networkConfiguration)
        {
            ConnectionManager connectionManager = CreateClientConnectionManager(networkConfiguration);
            Console.WriteLine(connectionManager);
            Console.WriteLine();
            return connectionManager;
        }

        public static void PressToExit()
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
            }
            while (keyInfo.Key != ConsoleKey.Enter);
        }

        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: Expected a script name but received none.");
                PressToExit();
                return;
            }

            else if (args.Length > 1)
            {
                Console.WriteLine("WARNING: Expected 1 argument but received " + (args.Length - 1) + ".");
                Console.WriteLine();
            }

            string username = args[0];
            string url = args[1];
            string[] protocolAndHostnameAndPort = url.Split("://");
            string[] hotnameAndPort = protocolAndHostnameAndPort[1].Split(":");
            int port = Int32.Parse(hotnameAndPort[1]);
            string filename = args[2];
            string networkConfiguration = args[3];

            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(filename);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("ERROR: File " + filename + " not found in current directory.");
                Console.WriteLine(e);
                PressToExit();
                return;
            }

            CommandDispatcher commandDispatcher = new CommandDispatcher();
            ConnectionManager connectionManager = CreateConnectionManager(networkConfiguration);
            RegisterCommands(commandDispatcher, connectionManager);

            try
            {
                Grpc.Core.Server server = new Grpc.Core.Server
                {
                    Services =
                    {
                        PuppetMasterClientService.BindService(new PuppetmasterClientServiceImpl())
                    },
                    Ports = { new ServerPort(hotnameAndPort[0], port, ServerCredentials.Insecure) }
                };
                Console.WriteLine("Client listening on port " + port);
                
                server.Start();

                List<string> preprocessed = CommandPreprocessor.Preprocess(lines);

                var timer = new Stopwatch();
                timer.Start();
                Task dispatcher = commandDispatcher.ExecuteAllAsync(preprocessed.ToArray());

                await dispatcher;
                timer.Stop();
                Console.WriteLine(timer.ElapsedMilliseconds);

                Console.WriteLine("Press ENTER to stop the client...");
                PressToExit();


                Console.WriteLine("\nShutting down...");
                server.ShutdownAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PressToExit();
                return;
            }
        }

        private static ConnectionManager CreateClientConnectionManager(string networkConfiguration)
        {
            InitializationParser initializationParser = new InitializationParser(networkConfiguration);
            List<Tuple<string, string>> serversConfiguration = initializationParser.getServersConfiguration();
            List<Tuple<string, List<string>>> partitionsConfiguration = initializationParser.getPartitionsConfiguration();

            IDictionary<string, Domain.Server> servers = new Dictionary<string, Domain.Server>();
            IDictionary<string, Partition> partitions = new Dictionary<string, Partition>();
            for (int i = 1; i < serversConfiguration.Count; i++)
            {
                Tuple<string, string> serverConfig = serversConfiguration[i];
                string serverId = serverConfig.Item1;
                string address = serverConfig.Item2;
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                GStoreService.GStoreServiceClient stub = new GStoreService.GStoreServiceClient(channel);
                Domain.Server server = new Domain.Server(serverId, stub);
                servers.Add(serverId, server);
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
                partitions.Add(partitionId, partition);
            }

            return new ConnectionManager(servers, partitions);
        }
    }
}
