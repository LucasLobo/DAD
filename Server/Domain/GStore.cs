using System;
using System.Collections.Concurrent;
using System.Threading;
using Utils;

namespace GStoreServer
{
    class GStore
    {
        private readonly ConcurrentDictionary<GStoreObjectIdentifier, GStoreObjectReplica> DataStore;
        private readonly ConcurrentDictionary<GStoreObjectIdentifier, ReaderWriterLockEnhancedSlim> ObjectLocks;

        public GStore()
        {
            DataStore = new ConcurrentDictionary<GStoreObjectIdentifier, GStoreObjectReplica>();
            ObjectLocks = new ConcurrentDictionary<GStoreObjectIdentifier, ReaderWriterLockEnhancedSlim>();
        }

        public void Write(GStoreObjectIdentifier gStoreObjectIdentifier, string newValue)
        {
            ReaderWriterLockEnhancedSlim objectLock = GetObjectLock(gStoreObjectIdentifier);
            int lockId = objectLock.EnterWriteLock();
            // send lock to replicas
            // await response
            // send update to replicas
            // await response

            AddOrUpdate(gStoreObjectIdentifier, newValue, true);

            objectLock.ExitWriteLock(lockId);
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

        public void ReadAll()
        {

        }

        public void GetIdSet()
        {

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

        private void AddOrUpdate(GStoreObjectIdentifier gStoreObjectIdentifier, string newValue, bool isMaster)
        {
            DataStore.AddOrUpdate(gStoreObjectIdentifier,
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
