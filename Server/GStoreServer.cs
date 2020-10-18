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

        public bool AddObject(GStoreObject gStoreObject)
        {
            return DataStore.TryAdd(gStoreObject.Identifier, gStoreObject);
        }

        public bool UpdateObject(GStoreObject gStoreObject)
        {
            if (DataStore.ContainsKey(gStoreObject.Identifier))
            {
                DataStore[gStoreObject.Identifier] = gStoreObject;
                return true;
            }
            return false;
        }

        public GStoreObject Read(GStoreObjectIdentifier identifier)
        {
            if (DataStore.TryGetValue(identifier, out GStoreObject obj))
            {
                return obj;
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
