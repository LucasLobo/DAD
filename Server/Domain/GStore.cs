using GStoreServer.Controllers;
using GStoreServer.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Utils;

namespace GStoreServer
{
    class GStore
    {
        private readonly ConcurrentDictionary<GStoreObjectIdentifier, GStoreObject> DataStore;
        private readonly ConcurrentDictionary<GStoreObjectIdentifier, ReaderWriterLockSlim> ObjectLocks;
        private readonly ConcurrentDictionary<GStoreObjectIdentifier, int> ObjectVersionNumber;
        private readonly ConcurrentDictionary<GStoreObjectIdentifier, GStoreObjectVersioning> ObjectVersionings;
        private readonly ConnectionManager connectionManager;

        public GStore(ConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            DataStore = new ConcurrentDictionary<GStoreObjectIdentifier, GStoreObject>();
            ObjectLocks = new ConcurrentDictionary<GStoreObjectIdentifier, ReaderWriterLockSlim>();
            ObjectVersionNumber = new ConcurrentDictionary<GStoreObjectIdentifier, int>();
            ObjectVersionings = new ConcurrentDictionary<GStoreObjectIdentifier, GStoreObjectVersioning>();
        }

        public void Write(GStoreObjectIdentifier gStoreObjectIdentifier, string newValue, int writeRequestId)
        {
            // Acquire lock on local object
            ReaderWriterLockSlim objectLock = GetObjectLock(gStoreObjectIdentifier);
            objectLock.EnterWriteLock();
            try
            {
                GStoreObject gStoreObject = AddOrUpdate(gStoreObjectIdentifier, newValue);

                // Send write requests to all replicas
                if (connectionManager.IsMasterForPartition(gStoreObjectIdentifier.PartitionId))
                {
                    int version = GetAndIncrementObjectVersionNumber(gStoreObjectIdentifier);
                    _ = WriteReplicaController.ExecuteAsync(connectionManager, gStoreObject, writeRequestId, version);
                }
                else
                {
                    // replica
                    GStoreObjectVersioning gStoreObjectVersioning = GetObjectVersioning(gStoreObjectIdentifier);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                objectLock.ExitWriteLock();
            }
        }

        public string Read(GStoreObjectIdentifier gStoreObjectIdentifier)
        {
            ReaderWriterLockSlim objectLock = GetObjectLock(gStoreObjectIdentifier);
            objectLock.EnterReadLock();
            try
            {
                string value = null;
                if (DataStore.TryGetValue(gStoreObjectIdentifier, out GStoreObject gStoreObject))
                {
                    value = gStoreObject.Value;
                }
                return value;
            }
            finally
            {
                objectLock.ExitReadLock();
            }
                      
        }

        public ICollection<GStoreObjectReplica> ReadAll()
        {
            ISet<GStoreObjectReplica> values = new HashSet<GStoreObjectReplica>();
            foreach (KeyValuePair<GStoreObjectIdentifier, GStoreObject> objectPair in DataStore)
            {
                GStoreObjectIdentifier gStoreObjectIdentifier = objectPair.Key;
                GStoreObject gStoreObject = objectPair.Value;
                ReaderWriterLockSlim objectLock = GetObjectLock(gStoreObjectIdentifier);

                objectLock.EnterReadLock();

                try
                {
                    string partitionId = gStoreObject.Identifier.PartitionId;
                    GStoreObjectReplica gStoreObjectReplica = new GStoreObjectReplica(gStoreObject, connectionManager.IsMasterForPartition(partitionId));
                    values.Add(gStoreObjectReplica);
                }
                finally
                {
                    objectLock.ExitReadLock();
                }
            }
            return values;
        }

        public void WriteReplica(GStoreObjectIdentifier gStoreObjectIdentifier, string newValue, int writeRequestId, int version)
        {
            ReaderWriterLockSlim objectLock = GetObjectLock(gStoreObjectIdentifier);
            objectLock.EnterWriteLock();
            try
            {
                GStoreObjectVersioning gStoreObjectVersioning = GetObjectVersioning(gStoreObjectIdentifier);
                AddOrUpdate(gStoreObjectIdentifier, newValue);
            }
            finally
            {
                objectLock.ExitWriteLock();
            }
        }

        public string GetMaster(string partitionId)
        {
            return connectionManager.GetPartitionMasterId(partitionId);
        }

        private int GetAndIncrementObjectVersionNumber(GStoreObjectIdentifier gStoreObjectIdentifier)
        {
            int version = ObjectVersionNumber.AddOrUpdate(gStoreObjectIdentifier,
                (key) =>
                {
                    return 0;
                },
                (key, value) =>
                {
                    return ++value;
                }
                );
            return version;
        }

        private GStoreObjectVersioning GetObjectVersioning(GStoreObjectIdentifier gStoreObjectIdentifier)
        {
            return ObjectVersionings.GetOrAdd(gStoreObjectIdentifier,
                (key) =>
                {
                    return new GStoreObjectVersioning();
                });
        }

        private ReaderWriterLockSlim GetObjectLock(GStoreObjectIdentifier gStoreObjectIdentifier)
        {
            return ObjectLocks.GetOrAdd(gStoreObjectIdentifier,
                (key) =>
                {
                    return new ReaderWriterLockSlim();
                });
        }

        private GStoreObject AddOrUpdate(GStoreObjectIdentifier gStoreObjectIdentifier, string newValue)
        {
            return DataStore.AddOrUpdate(gStoreObjectIdentifier,
                (id) =>
                {
                    return new GStoreObject(id, newValue);
                },
                (id, gStoreObject) =>
                {
                    gStoreObject.Value = newValue;
                    return gStoreObject;
                });
        }

        public ConnectionManager GetConnectionManager()
        {
            return connectionManager;
        }
    }
}
