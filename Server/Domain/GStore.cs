using GStoreServer.Controllers;
using GStoreServer.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace GStoreServer
{
    class GStore
    {
        private readonly ConcurrentDictionary<GStoreObjectIdentifier, GStoreObjectReplica> DataStore;
        private readonly ConcurrentDictionary<GStoreObjectIdentifier, ReaderWriterLockEnhancedSlim> ObjectLocks;
        private readonly ConnectionManager connectionManager;

        public GStore(ConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            DataStore = new ConcurrentDictionary<GStoreObjectIdentifier, GStoreObjectReplica>();
            ObjectLocks = new ConcurrentDictionary<GStoreObjectIdentifier, ReaderWriterLockEnhancedSlim>();
        }

        public async Task Write(GStoreObjectIdentifier gStoreObjectIdentifier, string newValue)
        {
            try
            {
                if (!connectionManager.IsMasterForPartition(gStoreObjectIdentifier.PartitionId))
                {
                    // throw not master error
                }

                // Acquire lock on local object
                ReaderWriterLockEnhancedSlim objectLock = GetObjectLock(gStoreObjectIdentifier);
                int lockId = objectLock.EnterWriteLock();

                // Get all replicas associated to this Partition
                ISet<Server> replicas = connectionManager.GetPartitionReplicas(gStoreObjectIdentifier.PartitionId);

                // Send lock requests to all remote objects
                IDictionary<string, Task<int>> lockTasks = new Dictionary<string, Task<int>>();
                foreach (Server replica in replicas)
                {
                    lockTasks.Add(replica.Id, LockController.Execute(replica.Stub, gStoreObjectIdentifier));
                }

                // Await for lock requests and save their values
                IDictionary<string, int> lockValues = new Dictionary<string, int>();
                foreach (KeyValuePair<string, Task<int>> lockTaskPair in lockTasks)
                {
                    lockValues.Add(lockTaskPair.Key, await lockTaskPair.Value);
                }

                // Once lock confirmations arrive, write to local object and unlock it
                GStoreObjectReplica gStoreObjectReplica = AddOrUpdate(gStoreObjectIdentifier, newValue, true);
                objectLock.ExitWriteLock(lockId);

                // Send write requests to all remote objects
                IDictionary<string, Task> writeTasks = new Dictionary<string, Task>();
                foreach (Server replica in replicas)
                {
                    writeTasks.Add(replica.Id, WriteReplicaController.Execute(replica.Stub, gStoreObjectReplica.Object, lockValues[replica.Id]));
                }

                // Await lock write requests
                foreach (KeyValuePair<string, Task> writeTaskPair in writeTasks)
                {
                    await writeTaskPair.Value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        public string Read(GStoreObjectIdentifier gStoreObjectIdentifier)
        {
            ReaderWriterLockEnhancedSlim objectLock = GetObjectLock(gStoreObjectIdentifier);
            string value = null;
            int id = objectLock.EnterReadLock();
            if (DataStore.TryGetValue(gStoreObjectIdentifier, out GStoreObjectReplica gStoreObjectReplica))
            {
                value = gStoreObjectReplica.Object.Value;
            }
            objectLock.ExitReadLock(id);
            return value;           
        }

        public async Task<ICollection<GStoreObjectReplica>> ReadAll()
        {
            ICollection<ReaderWriterLockEnhancedSlim> locks = ObjectLocks.Values;

            IDictionary<ReaderWriterLockEnhancedSlim, Task<int>> lockTasks = new Dictionary<ReaderWriterLockEnhancedSlim, Task<int>>();
            foreach (ReaderWriterLockEnhancedSlim objectLock in locks)
            {
                lockTasks.Add(objectLock, Task.Run(objectLock.EnterReadLock));
            }

            IDictionary<ReaderWriterLockEnhancedSlim, int> lockWithReadIdSet = new Dictionary<ReaderWriterLockEnhancedSlim, int>();

            foreach(KeyValuePair<ReaderWriterLockEnhancedSlim, Task<int>> lockTask in lockTasks)
            {
                ReaderWriterLockEnhancedSlim objectLock = lockTask.Key;
                Task<int> task = lockTask.Value;
                lockWithReadIdSet.Add(objectLock, await task);
            }

            ICollection<GStoreObjectReplica> values = DataStore.Values;

            foreach(KeyValuePair<ReaderWriterLockEnhancedSlim, int> lockWithReadId in lockWithReadIdSet)
            {
                ReaderWriterLockEnhancedSlim objectLock = lockWithReadId.Key;
                int lockId = lockWithReadId.Value;
                objectLock.ExitReadLock(lockId);
            }

            return values;
        }

        public async Task<ICollection<GStoreObjectIdentifier>> GetIdSet()
        {
            ICollection<ReaderWriterLockEnhancedSlim> locks = ObjectLocks.Values;

            IDictionary<ReaderWriterLockEnhancedSlim, Task<int>> lockTasks = new Dictionary<ReaderWriterLockEnhancedSlim, Task<int>>();
            foreach (ReaderWriterLockEnhancedSlim objectLock in locks)
            {
                lockTasks.Add(objectLock, Task.Run(objectLock.EnterReadLock));
            }

            IDictionary<ReaderWriterLockEnhancedSlim, int> lockWithReadIdSet = new Dictionary<ReaderWriterLockEnhancedSlim, int>();

            foreach (KeyValuePair<ReaderWriterLockEnhancedSlim, Task<int>> lockTask in lockTasks)
            {
                ReaderWriterLockEnhancedSlim objectLock = lockTask.Key;
                Task<int> task = lockTask.Value;
                lockWithReadIdSet.Add(objectLock, await task);
            }

            ICollection<GStoreObjectIdentifier> keys = DataStore.Keys;

            foreach (KeyValuePair<ReaderWriterLockEnhancedSlim, int> lockWithReadId in lockWithReadIdSet)
            {
                ReaderWriterLockEnhancedSlim objectLock = lockWithReadId.Key;
                int lockId = lockWithReadId.Value;
                objectLock.ExitReadLock(lockId);
            }

            return keys;
        }

        public int Lock(GStoreObjectIdentifier gStoreObjectIdentifier)
        {
            ReaderWriterLockEnhancedSlim objectLock = GetObjectLock(gStoreObjectIdentifier);
            return objectLock.EnterWriteLock();
        }

        public void WriteReplica(GStoreObjectIdentifier gStoreObjectIdentifier, string newValue, int lockId)
        {
            ReaderWriterLockEnhancedSlim objectLock = GetObjectLock(gStoreObjectIdentifier);

            if (!objectLock.IsWriteLockValid(lockId))
            {
                // throw error
            }

            AddOrUpdate(gStoreObjectIdentifier, newValue, false);

            objectLock.ExitWriteLock(lockId);
        }

        private ReaderWriterLockEnhancedSlim GetObjectLock(GStoreObjectIdentifier gStoreObjectIdentifier)
        {
            return ObjectLocks.GetOrAdd(gStoreObjectIdentifier,
                (key) =>
                {
                    return new ReaderWriterLockEnhancedSlim();
                });
        }

        private GStoreObjectReplica AddOrUpdate(GStoreObjectIdentifier gStoreObjectIdentifier, string newValue, bool isMaster)
        {
            return DataStore.AddOrUpdate(gStoreObjectIdentifier,
                (id) =>
                {
                    return new GStoreObjectReplica(new GStoreObject(id, newValue), isMaster);
                },
                (id, gStoreObjectReplica) =>
                {
                    gStoreObjectReplica.Object.Value = newValue;
                    return gStoreObjectReplica;
                });
        }

        public void ShowDataStore()
        {
            foreach (var item in DataStore)
            {
                Console.WriteLine(item.Value);
            }
        }
    }
}
