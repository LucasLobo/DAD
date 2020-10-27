using Google.Protobuf.WellKnownTypes;
using PuppetMaster.Controllers.PCSControllers;
using PuppetMaster.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class StatusCommand : Command
    {
        private TextBox txtBoxOutput;

        private readonly ConnectionManager ConnectionManager;
        public StatusCommand(TextBox output, ConnectionManager connectionManager) : base(true)
        {
            txtBoxOutput = output;
            ConnectionManager = connectionManager;
        }

        public static int EXPECTED_ARGUMENTS = 0;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                txtBoxOutput.AppendText(Environment.NewLine + "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            List<Empty> statusReplies = await StatusController.Execute(ConnectionManager);

            foreach(Empty reply in statusReplies)
            {
                txtBoxOutput.AppendText(Environment.NewLine + "=> " + reply.ToString());
            }

            txtBoxOutput.AppendText(Environment.NewLine + "Status DONE.");
        }
    }
}
