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
        public CreateServerCommand(TextBox output) : base(true)
        {
            txtBoxOutput = output;
        }

        public static int BASE_ARGUMENTS = 4;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            int serversNumber = Int32.Parse(arguments[0]);
            int MAX_ARGUMENTS = BASE_ARGUMENTS + serversNumber;
            if (arguments.Count != MAX_ARGUMENTS)
            {
                txtBoxOutput.AppendText(Environment.NewLine + "Expected a minimum of " + MAX_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            SystemConfiguration.GetInstance().AddServerConfig(string.Join(" ", arguments));
            txtBoxOutput.AppendText(Environment.NewLine + "Server Configurated.");
        }
    }
}
