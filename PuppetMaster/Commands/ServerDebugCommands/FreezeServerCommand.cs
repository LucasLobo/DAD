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
        private PuppetMasterServerServices.PuppetMasterServerServicesClient serverStub;
        public FreezeServerCommand(TextBox output, PuppetMasterServerServices.PuppetMasterServerServicesClient serverStub) : base(true)
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

            // Dummy implementation
            try
            {
                txtBoxOutput.AppendText(Environment.NewLine + "Freeze START.");
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
