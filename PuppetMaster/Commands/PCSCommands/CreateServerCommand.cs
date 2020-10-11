using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PuppetMaster.Commands
{
    class CreateServerCommand : Command
    {
        private TextBox txtBoxOutput;
        public CreateServerCommand(TextBox output)
        {
            this.txtBoxOutput = output;
        }

        public static int BASE_ARGUMENTS = 4;
        public override void Execute(List<string> arguments)
        {
            int serversNumber = Int32.Parse(arguments[0]);
            int MAX_ARGUMENTS = BASE_ARGUMENTS + serversNumber;
            if (arguments.Count != MAX_ARGUMENTS)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Expected a minimum of " + MAX_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            // Dummy implementation
            this.txtBoxOutput.AppendText(Environment.NewLine + "Partition DONE.");
        }
    }
}
