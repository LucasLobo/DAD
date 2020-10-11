using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PuppetMaster.Commands
{
    class UnfreezeServerCommand : Command
    {
        private TextBox txtBoxOutput;
        public UnfreezeServerCommand(TextBox output)
        {
            this.txtBoxOutput = output;
        }

        public static int EXPECTED_ARGUMENTS = 1;
        public override void Execute(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            // Dummy implementation
            this.txtBoxOutput.AppendText(Environment.NewLine + "Unfreeze DONE.");
        }
    }
}
