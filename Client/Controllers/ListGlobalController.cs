using Client.Domain;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Client.Controllers
{
    class ListGlobalController
    {

        public static async Task<HashSet<GStoreObjectIdentifier>> Execute(ConnectionManager connectionManager)
        {
            IDictionary<string, AsyncUnaryCall<GStoreListGlobalReply>> asyncUnaryCalls = new Dictionary<string, AsyncUnaryCall<GStoreListGlobalReply>>();

            foreach (Domain.Server server in connectionManager.GetAliveServers())
            {
                asyncUnaryCalls.Add(server.Id, server.Stub.ListGlobalAsync(new Empty()));
            }

            IDictionary<string, GStoreListGlobalReply> gStoreListGlobalReplies = new Dictionary<string, GStoreListGlobalReply>();
            foreach (KeyValuePair<string, AsyncUnaryCall<GStoreListGlobalReply>> request in asyncUnaryCalls)
            {
                try
                {
                    gStoreListGlobalReplies.Add(request.Key, await request.Value.ResponseAsync);
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Internal)
                {
                    Console.WriteLine($"Could not establish connection with server {request.Key}");
                }
            }

            HashSet<GStoreObjectIdentifier> gStoreObjectIdentifiers = new HashSet<GStoreObjectIdentifier>();

            foreach (KeyValuePair<string, GStoreListGlobalReply> reply in gStoreListGlobalReplies)
            {
                foreach (DataObjectIdentifier dataObjectIdentifier in reply.Value.ObjectIdentifiers)
                {
                    //Console.WriteLine($"({reply.Key}) {objectId.PartitionId},{objectId.ObjectId}");
                    gStoreObjectIdentifiers.Add(CreateObjectIdentifier(dataObjectIdentifier));
                }
            }

            return gStoreObjectIdentifiers;
        }

        private static GStoreObjectIdentifier CreateObjectIdentifier(DataObjectIdentifier dataObjectIdentifier)
        {
            return new GStoreObjectIdentifier(dataObjectIdentifier.PartitionId, dataObjectIdentifier.ObjectId);
        }
    }
}
