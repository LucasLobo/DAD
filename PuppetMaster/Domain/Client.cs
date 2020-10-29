using System;

namespace PuppetMaster.Domain
{
    class Client
    {
        public PuppetMasterClientService.PuppetMasterClientServiceClient Stub { get; }

        public Client(PuppetMasterClientService.PuppetMasterClientServiceClient client)
        {
            Stub = client ?? throw new ArgumentNullException("Client cannot be null.");
        }
    }
}
