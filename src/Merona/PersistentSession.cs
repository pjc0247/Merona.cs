﻿using System;
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

        static PersistentSession()
        {
            collectionNameCache = new Dictionary<Type, string>();
        }
        public PersistentSession()
        {
            collectionName = GetType().Name;
        }

        public Task Create(String key)
        {
            this.key = key;

            var database = Server.current.database;
            return Task.Factory.StartNew(async () =>
            {
                await database.GetCollection<BsonDocument>(collectionName).
                    InsertOneAsync(this.ToBsonDocument());
            });
        }
        public Task Open(String key)
        {
            var database = Server.current.database;

            return Task.Factory.StartNew(async () => {
                var filter = Builders<BsonDocument>.Filter.Eq("__merona_id", key);
                
                BsonDocument result = null;

                try {
                    /* TODO : */
                    result = await database.GetCollection<BsonDocument>(collectionName)
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
        public Task Commit()
        {
            var server = Server.current;

            return Task.Factory.StartNew(async () => {
                var filter = Builders<BsonDocument>.Filter.Eq("__merona_id", 10);
                
                /* TODO : */
                await server.database.GetCollection<BsonDocument>(collectionName)
                    .ReplaceOneAsync(filter, this.ToBsonDocument());
            });
        }
        public Task Remove()
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
