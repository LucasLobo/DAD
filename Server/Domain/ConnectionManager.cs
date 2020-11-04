using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Utils;

namespace GStoreServer.Domain
{
    class ConnectionManager : GenericConnectionManager<Server, MasterReplicaService.MasterReplicaServiceClient>
    {
        private string selfServerId;

        // Partitions in which this server is a Master
        private readonly ISet<string> masterPartitions;

        // Partitions in which this server is a Replica
        private readonly ISet<string> replicaPartitions;

        private static int heartbeatInterval = 10000;
        private static int gracePeriod = 2000;

        public ConnectionManager(IDictionary<string, Server> servers, IDictionary<string, Partition> partitions, ISet<string> masterPartitions, ISet<string> replicaPartitions, string selfServerId) : base(servers, partitions)
        {
            if (string.IsNullOrWhiteSpace(selfServerId))
            {
                throw new ArgumentException($"'{nameof(selfServerId)}' cannot be null or whitespace");
            }

            this.masterPartitions = masterPartitions ?? throw new ArgumentNullException(nameof(masterPartitions));
            this.replicaPartitions = replicaPartitions ?? throw new ArgumentNullException(nameof(replicaPartitions));
            this.selfServerId = selfServerId;
        }

        public ISet<Server> GetPartitionReplicas(string partitionId)
        {
            Partition partition = GetPartition(partitionId);

            ISet<Server> replicas = new HashSet<Server>();
            foreach (string replicaId in partition.ReplicaSet)
            {
                try
                {
                    Server server = GetServer(replicaId);
                    replicas.Add(GetServer(replicaId));
                }
                catch (Exception e)
                {
                    // fixme
                    Console.WriteLine(e.Message);
                }
            }
            return replicas;
        }

        public ISet<Server> GetMastersOfSelfReplicaPartitions()
        {
            ISet<Server> masters = new HashSet<Server>();
            foreach (string partitionId in replicaPartitions)
            {
                Partition partition = GetPartition(partitionId);
                Server server = GetServer(partition.MasterId);
                masters.Add(server);
            }
            return masters;
        }

        public bool IsMasterForPartition(string partitionId)
        {
            return masterPartitions.Contains(partitionId);
        }

        public new void DeclareDead(string serverId)
        {
            base.DeclareDead(serverId);

            // todo need to fix masterPartitions and replicaPartitions
        }

        public override string ToString()
        {
            string toString = base.ToString();

            toString += "\nPartitions where (self) master:";
            foreach(string partition in masterPartitions)
            {
                toString += " " + partition;
            }

            toString += "\nPartitions where (self) replica:";
            foreach (string partition in replicaPartitions)
            {
                toString += " " + partition;
            }

            return toString;
        }

        public async void StartSendingHeartbeats()
        {
            await Task.Delay(gracePeriod);

            while (true)
            {
                await SendHeartbeats();
                await Task.Delay(heartbeatInterval);
            }
        }

        private async Task SendHeartbeats()
        {
            IDictionary<string, Task> heartbeatTasks = new Dictionary<string, Task>();
            foreach (Domain.Server masterServer in GetMastersOfSelfReplicaPartitions())
            {
                heartbeatTasks.Add(masterServer.Id, SendHeartbeat(masterServer, selfServerId));
            }

            foreach (KeyValuePair<string, Task> heartbeatResponse in heartbeatTasks)
            {
                try
                {
                   await heartbeatResponse.Value;
                }
                catch (Grpc.Core.RpcException exception)
                {
                    if(exception.StatusCode == Grpc.Core.StatusCode.DeadlineExceeded || exception.StatusCode == Grpc.Core.StatusCode.Internal)
                    {
                        Console.WriteLine($"No response from server.ServerId: {heartbeatResponse.Key}");
                        DeclareDead(heartbeatResponse.Key);
                    }
                    else
                    {
                        Console.WriteLine($"Grpc Exception Occured");
                        Console.WriteLine(exception);
                    }
                }
            }
        }

        private async Task SendHeartbeat(Domain.Server server, string serverId)
        {
            await server.Stub.HeartBeatAsync(new HeartBeatRequest
            {
                ServerId = selfServerId
            },  deadline: DateTime.UtcNow.AddMilliseconds(heartbeatInterval));
        }
    }
}
