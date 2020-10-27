using PuppetMaster.Domain;
using System.Threading.Tasks;

namespace PuppetMaster.Controllers.PCSControllers
{
    class CreateClientController
    {
        public static async Task Execute(ConnectionManager connectionManager, string username, string clientURL, string scriptFile, string partitions)
        {
            PCS pcs = connectionManager.GetPCS(clientURL);
            // TODO create request and send it
            //pcs.Stub.Client(createClientRequest);
        }
    }
}
