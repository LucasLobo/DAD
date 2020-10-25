using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class FreezeServerCommand : Command
    {
        private TextBox txtBoxOutput;
        private PuppetMasterServerService.PuppetMasterServerServiceClient serverStub;
        public FreezeServerCommand(TextBox output, PuppetMasterServerService.PuppetMasterServerServiceClient serverStub) : base(true)
        {
            this.txtBoxOutput = output;
            this.serverStub = serverStub;
        }

        public static int EXPECTED_ARGUMENTS = 1;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                txtBoxOutput.AppendText(Environment.NewLine + "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            try
            {
                await serverStub.FreezeAsync(new Google.Protobuf.WellKnownTypes.Empty());
                txtBoxOutput.AppendText(Environment.NewLine + "Freeze DONE.");
            }
            catch (RpcException e)
            {
                txtBoxOutput.AppendText(Environment.NewLine + e.Message);
            }
        }
    }
}
