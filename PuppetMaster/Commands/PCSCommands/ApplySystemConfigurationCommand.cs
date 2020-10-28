using PuppetMaster.Controllers.PCSControllers;
using PuppetMaster.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands.PCSCommands
{
    class ApplySystemConfigurationCommand : Command
    {
        private readonly ConnectionManager ConnectionManager;

        private TextBox txtBoxOutput;
        public ApplySystemConfigurationCommand(TextBox output, ConnectionManager connectionManager) : base(true)
        {
            txtBoxOutput = output;
            ConnectionManager = connectionManager;
        }

        public override async Task ExecuteAsync(List<string> arguments)
        {
            List<string> serverLines = SystemConfiguration.GetInstance().GetServerLines();
            if (serverLines == null || serverLines.Count < 1)
            {
                txtBoxOutput.AppendText(Environment.NewLine + "No configuration provided. Please configure the system first.");
                return;
            }

            
            string servers = SystemConfiguration.GetInstance().GetServersArgument();
            string partitions = SystemConfiguration.GetInstance().GetPartitionsArgument();

            await ApplySystemConfigurationController.Execute(ConnectionManager, serverLines, servers, partitions);

            txtBoxOutput.AppendText(Environment.NewLine + "System Configuration Applied.");
        }
    }
}
