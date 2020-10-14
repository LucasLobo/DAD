using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class QuitCommand : Command
    {
        private TextBox txtBoxOutput;
        public QuitCommand(TextBox output)
        {
            this.txtBoxOutput = output;
        }

        public static int EXPECTED_ARGUMENTS = 0;
        public override void Execute(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine +  "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            Application.Exit();
        }
    }
}
