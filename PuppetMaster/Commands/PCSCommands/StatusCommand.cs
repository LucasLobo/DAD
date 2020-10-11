using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PuppetMaster.Commands
{
    class StatusCommand : Command
    {
        private TextBox txtBoxOutput;
        public StatusCommand(TextBox output)
        {
            this.txtBoxOutput = output;
        }

        public static int EXPECTED_ARGUMENTS = 0;
        public override void Execute(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            // Dummy implementation
            this.txtBoxOutput.AppendText(Environment.NewLine + "Status DONE.");
        }
    }
}
