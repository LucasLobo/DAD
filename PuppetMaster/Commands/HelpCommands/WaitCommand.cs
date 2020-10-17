using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using Utils;

namespace PuppetMaster.Commands
{
    class WaitCommand : Command
    {
        private TextBox txtBoxOutput;
        public WaitCommand(TextBox output) : base(false)
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

            try
            {
                int sleep = Int32.Parse(arguments.ElementAt(0));
                this.txtBoxOutput.AppendText(Environment.NewLine + "Sleeping...");
                await Task.Delay(sleep);
                this.txtBoxOutput.AppendText(Environment.NewLine + "Waking up...");
            }
            catch (FormatException)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Argument must be of type int.");
                return;
            }
        }
    }
}
