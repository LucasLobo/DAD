using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class ReplicationFactorCommand : Command
    {
        private TextBox txtBoxOutput;
        public ReplicationFactorCommand(TextBox output) : base(true)
        {
            txtBoxOutput = output;
        }

        public static int EXPECTED_ARGUMENTS = 1;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                txtBoxOutput.AppendText(Environment.NewLine + "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            txtBoxOutput.AppendText(Environment.NewLine + "Replication Factor Configured.");
        }
    }
}
