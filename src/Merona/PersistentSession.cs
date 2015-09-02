using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Merona
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Collection : Attribute
    {
        public String name { get; set; }

        public Collection(String name)
        {
            this.name = name;
        }
    }

    public class PersistentSession
    {
        public enum AutoCommitType
        {
            None,
            AfterRequest,
            AfterSessionClosed
        }

        private static Dictionary<Type,String> collectionNameCache { get; set; }

        private String collectionName { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        private ObjectId _id { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonElement("__merona_id")]
        private String key { get; set; }

        public String name { get; set; }
        public int num { get; set; }

        public bool isOpened
        {
            get
            {
                return true;
            }
        }
        public AutoCommitType autoCommitType { get; set; }

        static PersistentSession()
        {
            collectionNameCache = new Dictionary<Type, string>();
        }
        public PersistentSession(Server server)
        {
            this.autoCommitType = server.config.defaultPersistentSessionAutoCommitType;
            this.collectionName = GetType().Name;
        }

        public Task CreateAsync(String key)
        {
            // Create는 무조건 상속된 클래스에서 실행되어야 함
            if (GetType() == typeof(PersistentSession))
                throw new InvalidOperationException();

            this.key = key;

            var db = Server.current.database;

            db.GetCollection<BsonDocument>(collectionName);

            return new Task(() => { });
        }
        public Task OpenAsync(String key)
        {
            // Open은 무조건 상속된 클래스에서 실행되어야 함
            if (GetType() == typeof(PersistentSession))
                throw new InvalidOperationException();

            var server = Server.current;

            return Task.Factory.StartNew(async () => {
                var filter = Builders<BsonDocument>.Filter.Eq("__merona_id", key);

                Console.WriteLine("QQ");

                /* TODO : */

                BsonDocument result = null;
                Console.WriteLine("QQ");

                try {
                    result = await server.database.GetCollection<BsonDocument>(collectionName)
                    .Find(filter).FirstAsync();

                    var deserialized = MongoDB.Bson.Serialization.BsonSerializer.Deserialize(result, GetType());

                    foreach(var prop in deserialized.GetType().GetProperties())
                    {
                        Console.WriteLine(prop.Name);
                        prop.SetValue(this, prop.GetValue(deserialized));
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }

                Console.WriteLine(result);
            });
        }
        public Task CommitAsync()
        {
            var server = Server.current;

            return Task.Factory.StartNew(async () => {
                var filter = Builders<BsonDocument>.Filter.Eq("__merona_id", 10);
                
                /* TODO : */
                await server.database.GetCollection<BsonDocument>(collectionName)
                    .ReplaceOneAsync(filter, this.ToBsonDocument());
            });
        }
        public Task RemoveAsync()
        {
            var server = Server.current;

            return Task.Factory.StartNew(async () => {
                var filter = Builders<BsonDocument>.Filter.Eq("__merona_id", 10);

                /* TODO : */
                await server.database.GetCollection<BsonDocument>(collectionName)
                    .DeleteOneAsync(filter);
            });
        }
    }
}
