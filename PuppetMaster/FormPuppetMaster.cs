using Grpc.Net.Client;
using PuppetMaster.Commands;
using System;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster
{
    public partial class FormPuppetMaster : Form
    {

        static readonly CommandDispatcher commandDispatcher = new CommandDispatcher();

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
                isConcurrent = commandDispatcher.IsConcurrent(inputLine);
                if (isConcurrent) txtBoxCommand.ReadOnly = false;
                await commandDispatcher.ExecuteAsync(inputLine);
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
            string address = "http://localhost:8081";
            GrpcChannel channel = GrpcChannel.ForAddress(address);
            PuppetMasterServerServices.PuppetMasterServerServicesClient client = new PuppetMasterServerServices.PuppetMasterServerServicesClient(channel);

            // possible commands for configuration
            commandDispatcher.Register("replicationfactor", new ReplicationFactorCommand(txtBoxOutput));
            commandDispatcher.Register("partition", new PartitionCommand(txtBoxOutput));
            commandDispatcher.Register("server", new CreateServerCommand(txtBoxOutput));
            commandDispatcher.Register("client", new CreateClientCommand(txtBoxOutput));
            commandDispatcher.Register("status", new StatusCommand(txtBoxOutput));
            // possible commands for Replicas (debug)
            commandDispatcher.Register("crash", new CrashServerCommand(txtBoxOutput));
            commandDispatcher.Register("freeze", new FreezeServerCommand(txtBoxOutput, client));
            commandDispatcher.Register("unfreeze", new UnfreezeServerCommand(txtBoxOutput, client));
            // help commands
            commandDispatcher.Register("wait", new WaitCommand(txtBoxOutput));
            commandDispatcher.Register("help", new HelpCommand(txtBoxOutput));
            commandDispatcher.Register("clear", new ClearCommand(txtBoxOutput));
            commandDispatcher.Register("quit", new QuitCommand(txtBoxOutput));
        }
    }
}
