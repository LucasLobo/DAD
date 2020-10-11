using System;
using System.Collections.Generic;

namespace Utils
{
    public class CommandDispatcher
    {

        private readonly Dictionary<string, Command> commandMap = new Dictionary<string, Command>();

        public void Register(string commandName, Command command)
        {
            commandMap.Add(commandName, command);
        }

        public void Execute(string commandName, List<string> arguments)
        {
            commandMap.TryGetValue(commandName, out Command command);
            if (command == null)
            {
                Console.WriteLine("Unknown command. Use \"?\" for help."); //todo ?
                return;
            }
            command.Execute(arguments);
        }
    }
}
