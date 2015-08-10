using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Merona
{
    public class Model
    {
        public Model()
        {
        }

        private static Dictionary<List<Tuple<String, String>>, WeakReference<Model>> cache;

        static Model()
        {
            cache = new Dictionary<List<Tuple<String, String>>, WeakReference<Model>>();
        }

        internal static void Collect()
        {
            // RWLock or ConcurrentDic
            foreach(var item in cache)
            {
                Model target;

                if(!item.Value.TryGetTarget(out target))
                {
                    cache.Remove(item.Key);
                }
            }
        }

        public static async Task<Model> FindOneAsync<T>(Packet from) where T : Model, new()
        {
            var keys = new List<Tuple<String,String>>();

            foreach (var key in Packet.GetKeyFields(from.GetType()))
            {
                var value = (String)key.Item2.GetValue(from);

                keys.Add(new Tuple<String, String>(key.Item1, value));
            }

            if (cache.ContainsKey(keys))
            {
                Model cached;

                if (cache[keys].TryGetTarget(out cached))
                {
                    /* cache hit */

                    Console.WriteLine("cached data");

                    return cached;
                }
            }
            else
            {
                cache[keys] = new WeakReference<Model>(null);
            }

            /* cache miss */

            foreach (var key in keys)
            {
                Console.WriteLine(key.Item1);
                Console.WriteLine(key.Item2);
            }

            var result = new T();

            cache[keys].SetTarget(result);

            Console.WriteLine("load cache");

            //typeof(from).GetFields

            /*
            var collectionName = typeof(T).Name;
            var collection = Server.current.database.GetCollection<BsonDocument>(collectionName);

            Console.WriteLine("cOLLECTION NAME " + collectionName);

            var builder = Builders<BsonDocument>.Filter;

            var filter = builder.Eq("name", "aaa");

            var cursor = await collection.Find(filter).Limit(1).ToListAsync();

            foreach (var k in cursor)
            {
                foreach (var kv in k)
                {
                    Console.WriteLine(kv.Name);
                    Console.WriteLine(kv.Value);
                }
            }
            */

            return result;
        }
    }
}
