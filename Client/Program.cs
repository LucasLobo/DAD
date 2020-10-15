using System;
using System.Collections.Generic;
using System.Linq;
using Client.Commands;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Utils;

namespace Client
{

    class Program
    {

        static readonly CommandDispatcher commandDispatcher = new CommandDispatcher();

        private static void RegisterCommands()
        {
            commandDispatcher.Register("read", new ReadCommand());
            commandDispatcher.Register("write", new WriteCommand());
            commandDispatcher.Register("listServer", new ListServerCommand());
            commandDispatcher.Register("listGlobal", new ListGlobalCommand());
            commandDispatcher.Register("wait", new WaitCommand());
        }

        private static void ExecuteCommands(List<string> lines)
        {
            foreach (string line in lines)
            {
                List<string> splitLine = line.Split(' ').ToList();
                string command = splitLine.ElementAt(0);
                splitLine.RemoveAt(0);
                List<string> arguments = splitLine;

                Console.WriteLine(line);
                commandDispatcher.Execute(command, arguments);
                Console.WriteLine();
            };
        }

        /*static void Main(string[] args)
        {
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
            } catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("ERROR: File " + filename + " not found in current directory.");
                Console.WriteLine(e);
                return;
            }

            RegisterCommands();

            try
            {
                List<string> preprocessed = CommandPreprocessor.Preprocess(lines);
                ExecuteCommands(preprocessed);
            }
            catch (PreprocessingException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            ConnectionManager connectionManager = new ConnectionManager();
            connectionManager.PrintPartitions();

            Console.WriteLine(connectionManager.ChooseServerForWrite("part-1"));
            Console.WriteLine(connectionManager.ChooseServerForWrite("part-2"));
            Console.WriteLine(connectionManager.ChooseServerForWrite("part-3"));
            Console.WriteLine(connectionManager.ChooseServerForWrite("part-4"));
            Console.WriteLine(connectionManager.ChooseServerForWrite("part-5"));


            Console.WriteLine(connectionManager.ChooseServerForRead("part-1", "s-2"));
            Console.WriteLine(connectionManager.ChooseServerForRead("part-1", "s-1"));
            Console.WriteLine(connectionManager.ChooseServerForRead("part-1", "s-5"));
            Console.WriteLine(connectionManager.ChooseServerForRead("part-5", "s-5"));
            Console.WriteLine(connectionManager.ChooseServerForRead("part-3", "s-5"));
            //Console.WriteLine(connectionManager.ChooseServerForRead("part-1", "s-4"));
        }*/

        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:8081");
            GStoreService.GStoreServiceClient client = new GStoreService.GStoreServiceClient(channel);

            var reply = client.Read(new GStoreReadRequest { PartitionId = 1, ObjectId = 2});
            Console.WriteLine("Read:" + reply.ObjectValue);

            client.Write(new GStoreWriteRequest { PartitionId = 1, ObjectId = 2, ObjectValue = "value" });
            Console.WriteLine("Write");

            var reply3 = client.ListGlobal(new Empty());
            Console.WriteLine("ListGlobal:\n"+reply3.ObjectIdentifiers.ToString());

            var reply4 = client.ListServer(new Empty());
            Console.WriteLine("ListServer:\n"+reply4.Objects.ToString());

            Console.ReadKey();

        }
    }
}
