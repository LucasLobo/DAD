using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PuppetMaster;

namespace PuppetMaster.Commands
{
    class HelpCommand : Command
    {
         private TextBox txtBoxOutput;
         public HelpCommand(TextBox output)
         {
            this.txtBoxOutput = output;
         }

        public static int EXPECTED_ARGUMENTS = 0;
        public override void Execute(List<string> arguments)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            printHelpCommands();
        }

        private void printHelpCommands()
        {
            this.txtBoxOutput.AppendText(
                    Environment.NewLine + "Possible Commands for PCSs:" +
                    Environment.NewLine + "1. replicationfactor r" + 
                    Environment.NewLine + "2. server server_id URL min_delay max_delay" + 
                    Environment.NewLine + "3. partition r partition_name server_id_1 (...) server_id_r" + 
                    Environment.NewLine + "4. client username client_URL script_file" + 
                    Environment.NewLine + "5. status" + 
                    Environment.NewLine + "6. crash server_id" + 
                    Environment.NewLine + "7. freeze server_id" + 
                    Environment.NewLine + "8. unfreeze server_id" + 
                    Environment.NewLine + "9. clear" + 
                    Environment.NewLine + "10. quit"
                );
        }

    }
}
