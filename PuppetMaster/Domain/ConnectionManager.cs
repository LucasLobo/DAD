using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PuppetMaster.Domain
{
    class ConnectionManager
    {
        private readonly IDictionary<string, Server> serverSet;
        private readonly IDictionary<string, Client> clientSet;
        private readonly IDictionary<string, PCS> pcsSet;

        public ConnectionManager()
        {
            serverSet = new Dictionary<string, Server>();
            clientSet = new Dictionary<string, Client>();
            pcsSet = new Dictionary<string, PCS>();
        }

        // Set NEW Connections

        public void SetNewServerConnection(string serverId, string url)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            PuppetMasterServerService.PuppetMasterServerServiceClient client =
                   new PuppetMasterServerService.PuppetMasterServerServiceClient(channel);
            serverSet.Add(serverId, new Server(serverId, client));
        }

        public void SetNewClientConnection(string url)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            PuppetMasterClientService.PuppetMasterClientServiceClient client =
                new PuppetMasterClientService.PuppetMasterClientServiceClient(channel);
            clientSet.Add(url, new Client(client));
        }

        public void SetNewPCSConnection(string url)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            PuppetMasterPCSService.PuppetMasterPCSServiceClient client =
                new PuppetMasterPCSService.PuppetMasterPCSServiceClient(channel);
            pcsSet.Add(url, new PCS(client));
        }

        // Get Connections

        public List<Server> GetAllServerStubs()
        {
            return serverSet.Values.ToList();
        }

        public List<Client> GetAllClientStubs()
        {
            return clientSet.Values.ToList();
        }

        public List<PCS> GetAllPCSStubs()
        {
            return pcsSet.Values.ToList();
        }

        public Server GetServer(string serverId)
        {
            serverSet.TryGetValue(serverId, out Server server);
            if (server == null)
            {
                throw new NodeBindException("Server '" + serverId + "' not found.");

            }
            return server;
        }

        public Client GetClient(string clientURL)
        {
            clientSet.TryGetValue(clientURL, out Client client);
            if (client == null)
            {
                throw new NodeBindException("Client '" + clientURL + "' not found.");

            }
            return client;
        }

        public PCS GetPCS(string pcsURL)
        {
            pcsSet.TryGetValue(pcsURL, out PCS pcs);
            if (pcs == null)
            {
                throw new NodeBindException("PCS '" + pcsURL + "' not found.");

            }
            return pcs;
        }
    }

    [Serializable]
    public class NodeBindException : Exception
    {
        public NodeBindException()
        { }

        public NodeBindException(string message)
            : base(message)
        { }

        public NodeBindException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
