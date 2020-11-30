using Client.Domain;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Client.Controllers
{
    class WriteController
    {
        private static readonly Random random = new Random();
        public static async Task Execute(ConnectionManager connectionManager, string partitionId, string objectId, string value)
        {
            int id = random.Next(1, int.MaxValue);
            Console.WriteLine($"Write PartitionId: {partitionId} ObjectId: {objectId} Value: {value} WriteId: {id}");

            IImmutableSet<Domain.Server> servers = connectionManager.GetAliveServers(partitionId);
            GStoreWriteRequest writeRequest = new GStoreWriteRequest()
            {
                Object = new DataObject()
                {
                    ObjectIdentifier = new DataObjectIdentifier
                    {
                        PartitionId = partitionId,
                        ObjectId = objectId
                    },
                    Value = value
                },
                WriteRequestId = id
            };

            IDictionary<string, AsyncUnaryCall<Empty>> tasks = new Dictionary<string, AsyncUnaryCall<Empty>>();

            foreach (Domain.Server server in servers)
            {
                tasks.Add(server.Id, server.Stub.WriteAsync(writeRequest));
            }

            foreach (KeyValuePair<string, AsyncUnaryCall<Empty>> taskPair in tasks)
            {
                string serverId = taskPair.Key;
                AsyncUnaryCall<Empty> task = taskPair.Value;
                try
                {
                    await task;
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.Internal)
                {
                    await connectionManager.DeclareDead(serverId);
                }
            }
            Console.WriteLine("Write success");
        }
    }
}
