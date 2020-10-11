using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Linq;

namespace PuppetMaster.Commands
{
    class WaitCommand : Command
    {
        private TextBox txtBoxOutput;
        public WaitCommand(TextBox output)
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

            try
            {
                int milliseconds = Int32.Parse(arguments.ElementAt(0));
                this.txtBoxOutput.AppendText(Environment.NewLine + "Sleeping...");
                Thread.Sleep(milliseconds);
            }
            catch (FormatException)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Argument must be of type int.");
                return;
            }

        }
    }
}
