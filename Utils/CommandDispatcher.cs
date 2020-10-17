using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils
{
    public class CommandDispatcher
    {

        private readonly Dictionary<string, Command> commandMap = new Dictionary<string, Command>();

        public void Register(string commandName, Command command)
        {
            commandMap.Add(commandName, command);
        }


        public bool IsConcurrent(string line)
        {
            List<string> splitLine = new List<String>(line.Split(" "));
            string commandName = splitLine.ElementAt(0);
            splitLine.RemoveAt(0);

            Command command = GetCommand(commandName);
            return command.Concurrent;
        }

        public async Task ExecuteAsync(string line)
        {
            List<string> splitLine = new List<String>(line.Split(" "));
            string commandName = splitLine.ElementAt(0);
            splitLine.RemoveAt(0);

            Command command = GetCommand(commandName);
            await command.ExecuteAsync(splitLine);
        }


        public async Task ExecuteAllAsync(string[] lines)
        {
            List<Task> tasks = new List<Task>();
            foreach (String line in lines)
            {
                List<string> splitLine = new List<String>(line.Split(" "));
                string commandName = splitLine.ElementAt(0);
                splitLine.RemoveAt(0);

                Command command = GetCommand(commandName);

                if (command.Concurrent)
                {
                    Task task = command.ExecuteAsync(splitLine);
                    tasks.Add(task);
                }
                else
                {
                    await command.ExecuteAsync(splitLine);
                }
            }

            await Task.WhenAll(tasks);
        }

        private Command GetCommand(string commandName)
        {
            commandMap.TryGetValue(commandName, out Command command);
            if (command == null)
            {
                Console.WriteLine("Unknown command. Use \"?\" for help."); //todo ?
                return null;
            }
            return command;
        }
    }
}
