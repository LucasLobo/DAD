using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class ReplicationFactorCommand : Command
    {
        private TextBox txtBoxOutput;
        public ReplicationFactorCommand(TextBox output)
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
            this.txtBoxOutput.AppendText(Environment.NewLine + "Replication Factor DONE.");
        }
    }
}
