using System;

namespace Client.Domain
{
    class Server
    {
        public string Id { get; }

        public GStoreService.GStoreServiceClient Stub { get; }

        public Server(string id, GStoreService.GStoreServiceClient client)
        {
            Id = id ?? throw new ArgumentNullException("Server Id cannot be null.");
            Stub = client ?? throw new ArgumentNullException("Client cannot be null.");
        }
    }
}
