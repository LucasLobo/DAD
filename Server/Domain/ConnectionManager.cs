using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Utils;
using System.Timers;

namespace GStoreServer.Domain
{
    class ConnectionManager : GenericConnectionManager<Server, MasterReplicaService.MasterReplicaServiceClient>
    {
        private readonly string selfServerId;

        // Partitions in which this server is a Master
        private readonly ISet<string> masterPartitions;

        // Partitions in which this server is a Replica
        private readonly ISet<string> replicaPartitions;

        private IDictionary<string, Timer> replicasWatchdogs = new Dictionary<string, Timer>();

        private static readonly int HEARTBEAT_INTERVAL = 2000;

        private static readonly int GRACE_PERIOD = 2000;

        public ConnectionManager(IDictionary<string, Server> servers, IDictionary<string, Partition> partitions, string selfServerId) : base(servers, partitions)
        {
            if (string.IsNullOrWhiteSpace(selfServerId))
            {
                throw new ArgumentException($"'{selfServerId}' cannot be null or whitespace", nameof(selfServerId));
            }

            this.selfServerId = selfServerId;

            masterPartitions = new HashSet<string>();
            replicaPartitions = new HashSet<string>();
            foreach (Partition partition in partitions.Values)
            {
                if (partition.MasterId == selfServerId) masterPartitions.Add(partition.Id);
                else if (partition.ReplicaSet.Contains(selfServerId)) replicaPartitions.Add(partition.Id);
            }

            InitWatchdogs();
            StartSendingHeartbeats();
        }

        private async void InitWatchdogs()
        {
            await Task.Delay(GRACE_PERIOD);

            foreach (string partitionId in masterPartitions)
            {
                Partition serverPartition = GetPartition(partitionId);
                foreach (string serverId in serverPartition.ReplicaSet)
                {
                    replicasWatchdogs.Add(serverId, SetTimer(HEARTBEAT_INTERVAL + 1000, serverId));
                }
            }
        }

        public ISet<Server> GetMastersOfPartitionsWhereSelfReplica()
        {
            ISet<Server> masters = new HashSet<Server>();
            foreach (string partitionId in replicaPartitions)
            {
                Partition partition = GetPartition(partitionId);
                Server master = GetServer(partition.MasterId);
                masters.Add(master);
            }
            return masters;
        }

        public bool IsMasterForPartition(string partitionId)
        {
            lock(this)
            {
                return masterPartitions.Contains(partitionId);
            }
        }

        public new void DeclareDead(string deadServerId)
        {
            lock(this)
            {
                if (deadServerId == selfServerId)
                {
                    throw new Exception("Self Declared Dead");
                }

                base.DeclareDead(deadServerId);

                foreach (Partition partition in Partitions.Values)
                {
                    if (partition.MasterId == deadServerId)
                    {
                        List<string> sortedServerIds = partition.GetSortedServers();
                        int newMasterIndex = sortedServerIds.IndexOf(deadServerId);
                        string newMasterId;
                        do
                        {
                            newMasterIndex = (newMasterIndex + 1) % sortedServerIds.Count;
                            newMasterId = sortedServerIds.ElementAt(newMasterIndex);
                        } while (newMasterId != selfServerId && !GetServer(newMasterId).Alive);

                        ElectNewMaster(partition.Id, newMasterId);
                    }
                }
            }
        }


        // Caller ALWAYS should ensure mutual exclusion within this function
        private new void ElectNewMaster(string partitionId, string newMasterId)
        {
            // vvv Redundant vvv
            Partition partition = GetPartition(partitionId);
            if (partition.MasterId == selfServerId)
            {
                masterPartitions.Remove(partitionId);
                replicaPartitions.Add(partitionId);
            }
            // ^^^ Redudant ^^^

            base.ElectNewMaster(partitionId, newMasterId);

            if (newMasterId == selfServerId)
            {
                masterPartitions.Add(partitionId);
                replicaPartitions.Remove(partitionId);
            }
            
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

            toString += "\n=========================\n\n";

            return toString;
        }

        public async void StartSendingHeartbeats()
        {
            await Task.Delay(GRACE_PERIOD);
            
            while (true)
            {
                await SendHeartbeats();
                await Task.Delay(HEARTBEAT_INTERVAL);
            }
        }

        private async Task SendHeartbeats()
        {
            IDictionary<string, Task> heartbeatTasks = new Dictionary<string, Task>();
            foreach (Server masterServer in GetMastersOfPartitionsWhereSelfReplica())
            {
                heartbeatTasks.Add(masterServer.Id, SendHeartbeat(masterServer));
            }

            foreach (KeyValuePair<string, Task> heartbeatResponse in heartbeatTasks)
            {
                try
                {
                   await heartbeatResponse.Value;
                }
                catch (Grpc.Core.RpcException exception)
                {
                    //penso que com o freeze vai lançar a deadline exceeded e quando crasha lança a internal
                    if(exception.StatusCode == Grpc.Core.StatusCode.DeadlineExceeded || exception.StatusCode == Grpc.Core.StatusCode.Internal)
                    {
                        Console.WriteLine($"No response from server.ServerId: {heartbeatResponse.Key}");
                        DeclareDead(heartbeatResponse.Key);
                    }
                    else
                    {
                        Console.WriteLine($"Grpc Exception Occured");
                        Console.WriteLine(exception);
                        throw exception;
                    }
                }
            }
        }

        private async Task SendHeartbeat(Server server)
        {
            await server.Stub.HeartBeatAsync(new HeartBeatRequest
            {
                ServerId = selfServerId
            },  deadline: DateTime.UtcNow.AddMilliseconds(HEARTBEAT_INTERVAL));
        }

        public IDictionary<string, Timer> GetReplicasWatchDogs()
        {
            return replicasWatchdogs;
        }

        private Timer SetTimer(int timerInterval, string serverId)
        {
            // Create a timer with a two second interval.
            Timer timer = new Timer(timerInterval);
            // Hook up the Elapsed event for the timer. 
            timer.Elapsed += delegate { RemoveReplica(serverId); };
            timer.Enabled = true;
            return timer;
        }

        private void RemoveReplica(string serverId)
        {
            Console.WriteLine("Replica is dead -> " + serverId);
            //Stops and releases resources used by the timer
            replicasWatchdogs[serverId].Stop();
            replicasWatchdogs[serverId].Dispose();
            //Removes timer from watch dog list
            replicasWatchdogs.Remove(serverId);
            DeclareDead(serverId);
        }

        public void ResetTimer(Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
