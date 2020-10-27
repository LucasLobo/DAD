using PuppetMaster.Controllers.PCSControllers;
using PuppetMaster.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class CreateClientCommand : Command
    {
        private readonly ConnectionManager ConnectionManager;

        private TextBox txtBoxOutput;
        public CreateClientCommand(TextBox output, ConnectionManager connectionManager) : base(true)
        {
            txtBoxOutput = output;
            ConnectionManager = connectionManager;
        }

        public static int BASE_ARGUMENTS = 3;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            int serversNumber = Int32.Parse(arguments[0]);
            int MAX_ARGUMENTS = BASE_ARGUMENTS + serversNumber;
            if (arguments.Count != MAX_ARGUMENTS)
            {
                txtBoxOutput.AppendText(Environment.NewLine + "Expected a minimum of " + MAX_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            string username = arguments.ElementAt(0);
            string clientURL = arguments.ElementAt(1);
            string scriptFile = arguments.ElementAt(2);
            string partitions = SystemConfiguration.GetInstance().GetPartitionsArgument();

            await CreateClientController.Execute(ConnectionManager, username, clientURL, scriptFile, partitions);
            txtBoxOutput.AppendText(Environment.NewLine + "Client Created.");
        }
    }
}
