using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class FreezeServerCommand : Command
    {
        private TextBox txtBoxOutput;
        public FreezeServerCommand(TextBox output) : base(true)
        {
            this.txtBoxOutput = output;
        }

        public static int EXPECTED_ARGUMENTS = 1;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            // Dummy implementation
            this.txtBoxOutput.AppendText(Environment.NewLine + "Freeze DONE.");
        }
    }
}
