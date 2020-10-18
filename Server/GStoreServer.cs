using System;
using System.Collections.Concurrent;
using Utils;

namespace GStoreServer
{
    class GStoreServer
    {
        private ConcurrentDictionary<GStoreObjectIdentifier, GStoreObject> DataStore { get; }

        public GStoreServer()
        {
            GStoreObjectComparer comparer = new GStoreObjectComparer();
            DataStore = new ConcurrentDictionary<GStoreObjectIdentifier, GStoreObject>(comparer);
        }

        public bool AddObject(GStoreObject gstore_object)
        {
            return DataStore.TryAdd(gstore_object.Identifier, gstore_object);
        }

        public bool UpdateObject(GStoreObject gstore_object)
        {
            if (DataStore.ContainsKey(gstore_object.Identifier))
            {
                DataStore[gstore_object.Identifier] = gstore_object;
                return true;
            }
            return false;
        }

        public string ReadValue(GStoreObjectIdentifier identifier)
        {
            if (DataStore.TryGetValue(identifier, out GStoreObject obj))
            {
                return obj.Value;
            }
            return null;
        }

        public void ShowDataStore()
        {
            foreach (var item in DataStore)
            {
                Console.WriteLine(item.Key + "-" + item.Value);
            }
        }
    }
}
