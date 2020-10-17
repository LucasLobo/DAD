using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class CreateClientCommand : Command
    {
        private TextBox txtBoxOutput;
        public CreateClientCommand(TextBox output) : base(true)
        {
            this.txtBoxOutput = output;
        }

        public static int BASE_ARGUMENTS = 3;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            int serversNumber = Int32.Parse(arguments[0]);
            int MAX_ARGUMENTS = BASE_ARGUMENTS + serversNumber;
            if (arguments.Count != MAX_ARGUMENTS)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Expected a minimum of " + MAX_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            // Dummy implementation
            this.txtBoxOutput.AppendText(Environment.NewLine + "Creating client...");
            await Task.Delay(1000);
            this.txtBoxOutput.AppendText(Environment.NewLine + "Done creating client...");
        }
    }
}
