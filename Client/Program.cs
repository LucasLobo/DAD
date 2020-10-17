using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Commands;
using Client.Domain;
using Google.Protobuf.WellKnownTypes;
using Utils;

namespace Client
{

    class Program
    {

        static readonly CommandDispatcher commandDispatcher = new CommandDispatcher();

        private static void RegisterCommands(ConnectionManager connectionManager)
        {
            commandDispatcher.Register("read", new ReadCommand(connectionManager));
            commandDispatcher.Register("write", new WriteCommand(connectionManager));
            commandDispatcher.Register("listServer", new ListServerCommand(connectionManager));
            commandDispatcher.Register("listGlobal", new ListGlobalCommand(connectionManager));
            commandDispatcher.Register("wait", new WaitCommand());
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

            ConnectionManager connectionManager = new ConnectionManager();
            connectionManager.PrintPartitions();
            Console.WriteLine();
            Console.WriteLine();

            RegisterCommands(connectionManager);

            try
            {
                List<string> preprocessed = CommandPreprocessor.Preprocess(lines);
                Task dispatcher = commandDispatcher.ExecuteAllAsync(preprocessed.ToArray());

                for (int i = 0; i < 25; i++)
                {
                    Console.WriteLine("---");
                    await Task.Delay(500);
                }

                await dispatcher;
            }
            catch (PreprocessingException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            //Testing();
        }

        public static void Testing(ConnectionManager connectionManager)
        {

            Console.WriteLine(connectionManager.ChooseServerForWrite("part-1").Id);
            Console.WriteLine(connectionManager.ChooseServerForWrite("part-2").Id);
            Console.WriteLine(connectionManager.ChooseServerForWrite("part-3").Id);
            Console.WriteLine(connectionManager.ChooseServerForWrite("part-4").Id);
            Console.WriteLine(connectionManager.ChooseServerForWrite("part-5").Id);


            Console.WriteLine(connectionManager.ChooseServerForRead("part-1", "s-2").Id);
            Console.WriteLine(connectionManager.ChooseServerForRead("part-1", "s-1").Id);
            Console.WriteLine(connectionManager.ChooseServerForRead("part-1", "s-5").Id);
            Console.WriteLine(connectionManager.ChooseServerForRead("part-5", "s-5").Id);
            Console.WriteLine(connectionManager.ChooseServerForRead("part-3", "s-5").Id);

            Server server = connectionManager.ChooseServerForWrite("part-1");
            GStoreListGlobalReply gStoreListGlobalReply = server.Stub.ListGlobal(new Empty());

            Console.WriteLine(gStoreListGlobalReply.ObjectIdentifiers.ToString());
        }
    }
}
