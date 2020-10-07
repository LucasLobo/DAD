using System;
using System.Collections.Generic;

namespace Client.Domain
{
    class ConnectionManager
    {
        private readonly IDictionary<string, Partition> partitionSet;

        private string attachedServerId;

        public ConnectionManager()
        {
            partitionSet = new Dictionary<string, Partition>();
            CreatePartitions();
        }

        private void CreatePartitions()
        {

            int serverNumber = 5;
            int partitionSize = 3;

            for (int i = 1; i <= serverNumber; i++)
            {
                ISet<string> serverSet = new HashSet<string>();

                for (int j = 0; j < partitionSize; j++)
                {
                    int serverId = (i + j - 1) % serverNumber + 1;
                    serverSet.Add("s-" + serverId);
                }
                Partition partition = new Partition("part-" + i, "s-" + i, serverSet);
                partitionSet.Add("part-" + i, partition);
            }
        }

        public void PrintPartitions()
        {
            foreach (KeyValuePair<string, Partition> entry in partitionSet)
            {
                entry.Value.Print();
                Console.WriteLine();
            }
        }


        public string ChooseServerForRead(string partitionId, string serverId)
        {
            Partition partition = getPartition(partitionId);

            // No server currently attached
            if (String.IsNullOrEmpty(attachedServerId))
            {
                if (serverId.Equals("-1"))
                {
                    throw new ServerBindException("Not attached to any server and no default server is provided.");
                }
                else if (!partition.Contains(serverId))
                {
                    throw new ServerBindException("Default server '" + serverId + "' is not part of partition '" + partitionId + "'.");  
                }
                else
                {
                    attachedServerId = serverId;
                }
            }

            else if (!partition.Contains(attachedServerId))
            {
                if (serverId.Equals("-1"))
                {
                    throw new ServerBindException("Attached server '" + attachedServerId + "' is not part of the partition '" + partitionId + "' and no default server is provided.");
                }
                else if (!partition.Contains(serverId))
                {
                    throw new ServerBindException("Default server '" + serverId + "' is not part of partition '" + partitionId + "'.");
                }
                else
                {
                    attachedServerId = serverId;
                }
            }

            return attachedServerId;
        }

        public string ChooseServerForWrite(string partitionId)
        {
            Partition partition = getPartition(partitionId);

            // should it attach itself?
            // attachedServerId = partition.MasterId;
            return partition.MasterId;
        }

        private Partition getPartition(string partitionId)
        {
            partitionSet.TryGetValue(partitionId, out Partition partition);
            if (partition == null)
            {
                throw new ServerBindException("Partition '" + partitionId + "' not found.");
            }
            return partition;
        }

    }

    [Serializable]
    public class ServerBindException : Exception
    {
        public ServerBindException()
        { }

        public ServerBindException(string message)
            : base(message)
        { }

        public ServerBindException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
