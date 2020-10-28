using PuppetMaster.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class CreateServerCommand : Command
    {
        private TextBox txtBoxOutput;
        private ConnectionManager ConnectionManager;
        public CreateServerCommand(TextBox output, ConnectionManager connectionManager) : base(true)
        {
            txtBoxOutput = output;
            ConnectionManager = connectionManager;
        }

        public static int EXPECTED_ARGUMENTS = 4;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                txtBoxOutput.AppendText(Environment.NewLine + "Expected a minimum of " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }
            
            SystemConfiguration.GetInstance().AddServerConfig(string.Join(" ", arguments));
            ConnectionManager.SetNewServerConnection(arguments[0], arguments[1]);
            txtBoxOutput.AppendText(Environment.NewLine + "Server Configurated.");
        }
    }
}
