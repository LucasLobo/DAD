using System;
using System.Collections.Generic;
using System.Linq;
using static Utils.ServerBindException;

namespace Utils
{
    public abstract class GenericConnectionManager<TServer, T> where TServer : GenericServer<T>
    {
        private IDictionary<string, TServer> Servers { get; }

        private IDictionary<string, Partition> Partitions { get; }

        protected GenericConnectionManager(IDictionary<string, TServer> servers, IDictionary<string, Partition> partitions)
        {
            Servers = servers;
            Partitions = partitions;
        }

        public override string ToString()
        {
            string lines = "Servers:\n";

            foreach (KeyValuePair<string, TServer> serverEntry in Servers)
            {
                lines += $"  {serverEntry.Value}\n";
            }

            lines += "\nPartitions:\n";
            foreach (KeyValuePair<string, Partition> partitionEntry in Partitions)
            {
                lines += $"  {partitionEntry.Value}\n";
            }

            return lines;
        }

        public Partition GetPartition(string partitionId)
        {
            if (string.IsNullOrWhiteSpace(partitionId))
            {
                throw new ArgumentException($"'{nameof(partitionId)}' cannot be null or whitespace");
            }

            Partitions.TryGetValue(partitionId, out Partition partition);
            if (partition == null)
            {
                throw new ArgumentException($"Partition '{nameof(partitionId)}' not found.");
            }
            return partition;
        }

        public TServer GetServer(string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentException($"'{nameof(serverId)}' cannot be null or whitespace");
            }

            Servers.TryGetValue(serverId, out TServer server);
            if (server == null)
            {
                throw new ArgumentException($"Server '{nameof(serverId)}' not found.");
            }

            if (!server.Alive)
            {
                throw new ServerBindException($"Server '{serverId}' is dead.", ServerBindExceptionStatus.SERVER_DEAD);
            }

            return server;
        }

        public ISet<TServer> GetServers()
        {
            return Servers.Values.ToHashSet();
        }

        public void DeclareDead(string serverId)
        {
            TServer server = GetServer(serverId);
            server.DeclareDead();
        }
    }

    [Serializable]
    public class ServerBindException : Exception
    {

        public enum ServerBindExceptionStatus
        {
            DEFAULT,
            SERVER_DEAD
        }

        public ServerBindExceptionStatus Status { get; }

        public ServerBindException() => Status = ServerBindExceptionStatus.DEFAULT;

        public ServerBindException(string message, ServerBindExceptionStatus status = ServerBindExceptionStatus.DEFAULT)
            : base(message) => Status = status;

        public ServerBindException(string message, Exception innerException, ServerBindExceptionStatus status = ServerBindExceptionStatus.DEFAULT)
            : base(message, innerException) => Status = status;


    }
}

