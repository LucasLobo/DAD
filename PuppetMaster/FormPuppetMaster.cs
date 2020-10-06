using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class FormPuppetMaster : Form
    {

        // possible commands
        private const string COMMAND_REPLICATION = "replicationfactor";
        private const string COMMAND_PARTITION   = "partition";
        private const string COMMAND_SERVER      = "server";
        private const string COMMAND_CLIENT      = "client";
        private const string COMMAND_STATUS      = "status";

        // possible commands for Replicas (debug)
        private const string COMMAND_CRASH    = "crash";
        private const string COMMAND_FREEZE   = "freeze";
        private const string COMMAND_UNFREEZE = "unfreeze";

        // help commands
        private const string COMMAND_HELP  = "help";
        private const string COMMAND_CLEAR = "clear";
        private const string COMMAND_QUIT  = "quit";

        public FormPuppetMaster()
        {
            InitializeComponent();
        }

        private void txtBoxCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            // press <Enter> is the same as click in 'Run Command'
            if (e.KeyChar == (char)13)
            {
                btnRunCommand_Click(sender, e);
            }
        }

        private void btnRunScript_Click(object sender, EventArgs e)
        {
            //TODO
        }

        private void btnRunCommand_Click(object sender, EventArgs e)
        {
            string[] inputArguments = txtBoxCommand.Text.ToLower().Split(" ");

            switch (inputArguments[0])
            {

                case COMMAND_REPLICATION:
                {
                    string output = PuppetMasterDomain.ReplicationFactorCommand(inputArguments[1]);
                    printOutput(output);
                }
                break;

                case COMMAND_PARTITION:
                {
                    List<string> servers_id = new List<string>();
                    for (int iServer = 3; iServer < inputArguments.Length; iServer++)
                        servers_id.Add(inputArguments[iServer]);

                    string output = PuppetMasterDomain.PartitionCommand(inputArguments[1], inputArguments[2], servers_id);
                    printOutput(output);
                }
                break;

                case COMMAND_SERVER:
                {
                    string output = PuppetMasterDomain.ServerCommand(inputArguments[1], inputArguments[2], inputArguments[3], inputArguments[4]);
                    printOutput(output);
                }
                break;

                case COMMAND_CLIENT:
                {
                    string output = PuppetMasterDomain.ClientCommand(inputArguments[1], inputArguments[2], inputArguments[3]);
                    printOutput(output);
                }
                break;

                case COMMAND_STATUS:
                {
                    string output = PuppetMasterDomain.StatusCommand();
                    printOutput(output);
                }
                break;

                // possible commands for Replicas (debug)
                case COMMAND_CRASH:
                {
                    string output = PuppetMasterDomain.CrashCommand(inputArguments[1]);
                    printOutput(output);
                }
                break;

                case COMMAND_FREEZE:
                {
                    string output = PuppetMasterDomain.FreezeCommand(inputArguments[1]);
                    printOutput(output);
                }
                break;

                case COMMAND_UNFREEZE:
                {
                    string output = PuppetMasterDomain.UnfreezeCommand(inputArguments[1]);
                    printOutput(output);
                }
                break;

                // help commands
                case COMMAND_HELP:
                    printHelpCommands();
                break;

                case COMMAND_CLEAR:
                    clearOutputBox();
                break;

                case COMMAND_QUIT:
                    Application.Exit();
                break;

                // command error
                default:
                    printCommandError();
                break;

            }

        }

        private void printHelpCommands()
        {
            txtBoxOutput.AppendText(Environment.NewLine + "Possible Commands for PCSs:");
            txtBoxOutput.AppendText(Environment.NewLine + "1. " + COMMAND_REPLICATION + " r");
            txtBoxOutput.AppendText(Environment.NewLine + "2. " + COMMAND_SERVER + " server_id URL min_delay max_delay");
            txtBoxOutput.AppendText(Environment.NewLine + "3. " + COMMAND_PARTITION + " r partition_name server_id_1 (...) server_id_r");
            txtBoxOutput.AppendText(Environment.NewLine + "4. " + COMMAND_CLIENT + " username client_URL script_file");
            txtBoxOutput.AppendText(Environment.NewLine + "5. " + COMMAND_STATUS);
            txtBoxOutput.AppendText(Environment.NewLine + "6. " + COMMAND_CRASH + " server_id");
            txtBoxOutput.AppendText(Environment.NewLine + "7. " + COMMAND_FREEZE + " server_id");
            txtBoxOutput.AppendText(Environment.NewLine + "8. " + COMMAND_UNFREEZE + " server_id");
            txtBoxOutput.AppendText(Environment.NewLine + "9. clear");
            txtBoxOutput.AppendText(Environment.NewLine + "10. quit");
        }

        private void printCommandError()
        {
            txtBoxOutput.AppendText(Environment.NewLine + "Command invalid. If you need help use the command 'help'");
        }

        private void printOutput(string output)
        {
            txtBoxOutput.AppendText(Environment.NewLine + output);
        }

        private void clearOutputBox()
        {
            txtBoxOutput.Clear();
        }
    }
}
