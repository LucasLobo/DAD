using System.Collections.Generic;

namespace PuppetMaster.Domain
{
    class SystemConfiguration
    {
        private static readonly object Instancelock = new object();
        private static SystemConfiguration Instance = null;

        private static List<string> partitionLines;
        private static List<string> serverLines;

        private SystemConfiguration()
        {
            partitionLines = new List<string>();
            serverLines = new List<string>();
        }

        public static SystemConfiguration GetInstance()
        {
            if (Instance != null) return Instance;
            // lazy implementation
            lock (Instancelock) { Instance = new SystemConfiguration(); }
            return Instance;
        }

        public void AddPartitionConfig(string partitionConfigLine)
        {
            partitionLines.Add(partitionConfigLine);
        }

        public void AddServerConfig(string serverConfigLine)
        {
            serverLines.Add(serverConfigLine);
        }

        public string GetPartitionsArgument()
        {
            return " -p " + string.Join(" -p ", partitionLines);
        }

        public List<string> GetSystemConfiguration()
        {
            List<string> configurationLines = new List<string>();
            foreach (string serverLine in serverLines)
            {
                configurationLines.Add(serverLine + GetPartitionsArgument());
            }
            return configurationLines;
        }

    }
}
