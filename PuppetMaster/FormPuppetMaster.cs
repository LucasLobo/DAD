using Grpc.Net.Client;
using PuppetMaster.Commands;
using PuppetMaster.Commands.PCSCommands;
using PuppetMaster.Domain;
using System;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster
{
    public partial class FormPuppetMaster : Form
    {

        private static readonly CommandDispatcher CommandDispatcher = new CommandDispatcher();
        private static readonly ConnectionManager ConnectionManager = new ConnectionManager();

        // always starts with the configuration
        private static bool isConfiguring = true;

        public FormPuppetMaster()
        {
            InitializeComponent();
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            RegisterCommands();
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

        private async void btnRunCommand_Click(object sender, EventArgs e)
        {
            string inputLine = txtBoxCommand.Text.ToLower();
            if (String.IsNullOrEmpty(inputLine)) return;

            // clean the command textbox
            txtBoxCommand.ReadOnly = true;
            txtBoxCommand.Clear();
            bool isConcurrent;
            try
            {
                isConcurrent = CommandDispatcher.IsConcurrent(inputLine);
                if (isConcurrent) txtBoxCommand.ReadOnly = false;
                //hard style
                string commandName = CommandDispatcher.ExtractCommandName(inputLine);
                if (isConfiguring && CommandDispatcher.IsValidCommand(commandName) && !commandName.Equals("partition") && !commandName.Equals("server")
                    && !commandName.Equals("help") && !commandName.Equals("wait") && !commandName.Equals("quit") && !commandName.Equals("clear"))
                {
                    isConfiguring = false;
                    await CommandDispatcher.ExecuteAsync("applysystemconfiguration");
                }
                await CommandDispatcher.ExecuteAsync(inputLine);
            }
            catch (PreprocessingException exception)
            {
                txtBoxOutput.AppendText(Environment.NewLine + exception.Message);
                return;
            }
            catch (CommandNotRegisteredException exception)
            {
                txtBoxOutput.AppendText(Environment.NewLine + exception.Message);
            }
            finally
            {
                txtBoxCommand.ReadOnly = false;
            }
        }

        private void RegisterCommands()
        {
            // possible commands for configuration
            CommandDispatcher.Register("replicationfactor", new ReplicationFactorCommand(txtBoxOutput));
            CommandDispatcher.Register("partition", new PartitionCommand(txtBoxOutput));
            CommandDispatcher.Register("server", new CreateServerCommand(txtBoxOutput, ConnectionManager));
            CommandDispatcher.Register("client", new CreateClientCommand(txtBoxOutput, ConnectionManager));
            CommandDispatcher.Register("status", new StatusCommand(txtBoxOutput, ConnectionManager));
            CommandDispatcher.Register("applysystemconfiguration", new ApplySystemConfigurationCommand(txtBoxOutput, ConnectionManager));
            // possible commands for Replicas (debug)
            CommandDispatcher.Register("crash", new CrashServerCommand(txtBoxOutput, ConnectionManager));
            CommandDispatcher.Register("freeze", new FreezeServerCommand(txtBoxOutput, ConnectionManager));
            CommandDispatcher.Register("unfreeze", new UnfreezeServerCommand(txtBoxOutput, ConnectionManager));
            // help commands
            CommandDispatcher.Register("wait", new WaitCommand(txtBoxOutput));
            CommandDispatcher.Register("help", new HelpCommand(txtBoxOutput));
            CommandDispatcher.Register("clear", new ClearCommand(txtBoxOutput));
            CommandDispatcher.Register("quit", new QuitCommand(txtBoxOutput));
        }
    }
}
