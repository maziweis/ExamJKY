using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CommonHelper
{
    
    public class MongoDbHelper
    {
        public static string dbName= System.Configuration.ConfigurationManager.AppSettings["dbName"];
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected static IMongoCollection<BsonDocument> _collection;

        private static IMongoClient client
        {
            get
            {
                if (null == _client)
                {
                    _client = new MongoClient(ConfigurationManager.ConnectionStrings["jky_db"].ToString());
                }
                return _client;
            }
        }

        private static IMongoDatabase database
        {
            get
            {
                _database = client.GetDatabase(dbName);
                return _database;
            }
        }

        private static IMongoCollection<BsonDocument> collection
        {
            get
            {
                return _collection;
            }
            set
            {
                _collection = value;
            }
        }

        /// <summary>
        /// Insert数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static void Insert<T>(T model, string tableName) where T : new()
        {
            var m = new BsonDocument(model.ToBsonDocument());
            collection = database.GetCollection<BsonDocument>(tableName);
            collection.InsertOne(m);
        }

        /// <summary>
        /// Insert数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static void Insert<T>(List<T> models, string tableName) where T : new()
        {
            List<BsonDocument> docunemts = new List<BsonDocument>();
            collection = database.GetCollection<BsonDocument>(tableName);

            if (models != null && models.Count > 0)
            {
                foreach (var item in models)
                {
                    var m = new BsonDocument(item.ToBsonDocument());
                    docunemts.Add(m);
                }
                collection.InsertMany(docunemts);
            }
        }

        /// <summary>
        /// 更新数据库(单条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        public static void ReplaceOne<T>(string id, T model, string tableName) where T : new()
        {
            ObjectId objectid = new ObjectId();
            if (ObjectId.TryParse(id, out objectid))
            {
                var filter = new BsonDocument();
                filter.Add("_id", objectid);

                var m = new BsonDocument(model.ToBsonDocument());
                collection = database.GetCollection<BsonDocument>(tableName);
                collection.ReplaceOne(filter, m);
            }
        }
        /// <summary>
        /// 更新数据库(单条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        public static void ReplaceOne1<T>(string id, T model, string tableName) where T : new()
        {
            var filter = new BsonDocument();
            filter.Add("_id", id);
            var m = new BsonDocument(model.ToBsonDocument());
            collection = database.GetCollection<BsonDocument>(tableName);
            collection.ReplaceOne(filter, m);
        }
        /// <summary>
        ///  删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        public static void DeleteOne<T>(string id, string tableName) where T : new()
        {
            ObjectId objectid = new ObjectId();
            if (ObjectId.TryParse(id, out objectid))
            {
                var filter = new BsonDocument();
                filter.Add("_id", objectid);
                database.GetCollection<T>(tableName).DeleteOne(filter);
            }
        }
        public static void DeleteByBson<T>(BsonDocument filter, string tableName) where T : new()
        {
            database.GetCollection<T>(tableName).DeleteMany(filter);
        }
        public static void DeleteAll(string tableName)
        {
            database.DropCollection(tableName);
        }
        /// <summary>
        /// 根据主键查询一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static T FindOne<T>(string id, string tableName) where T : new()
        {
            ObjectId objectid = new ObjectId();
            if (ObjectId.TryParse(id, out objectid))
            {
                var filter = new BsonDocument();
                filter.Add("_id", objectid);
                return database.GetCollection<T>(tableName).Find(filter).FirstOrDefault();
            }
            return default(T);
        }
        public static List<T> FindByBson<T>(BsonDocument filter, string tableName) where T : new()
        {
            return database.GetCollection<T>(tableName).Find(filter).ToList();
        }
        public static T QueryOne<T>(string tableName, Expression<Func<T, bool>> where)
        {
            return database.GetCollection<T>(tableName).AsQueryable().Where(where).AsEnumerable().FirstOrDefault();
        }
        public static List<T> QueryBy<T>(string tableName, Expression<Func<T, bool>> where)
        {
            return database.GetCollection<T>(tableName).AsQueryable().Where(where).AsEnumerable().ToList();
        }
        public static List<T> FindAll<T>(string tableName) where T : new()
        {
            return database.GetCollection<T>(tableName).AsQueryable().Where(w => 1 == 1).AsEnumerable().ToList();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">表名类</typeparam>
        /// <typeparam name="Tk">表字段</typeparam>
        /// <param name="collectionName">表名</param>
        /// <param name="skip">跳过多少条</param>
        /// <param name="limit">查询多少条</param>
        /// <param name="whereLambda">条件</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public static List<T> GetPagedList<T, Tk>(string collectionName, int skip, int limit, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tk>> orderBy)
        {
            var collection = database.GetCollection<T>(collectionName);
            //var filter = collection == null ? FilterDefinition<T>.Empty : Builders<T>.Filter.Where(whereLambda);
            // 分页 一定注意： Skip 之前一定要 OrderBy
            var cursor = collection.AsQueryable().Where(whereLambda).OrderByDescending(orderBy).Skip(skip).Take(limit);
            //var cursor = collection.Find(whereLambda).SortByDescending(orderBy);
            return cursor.AsEnumerable().ToList();
        }
        public static List<T> GetPagedList1<T, Tk>(string collectionName, int skip, int limit, BsonDocument bson, Expression<Func<T, object>> orderBy)
        {
            var collection = database.GetCollection<T>(collectionName);
            return collection.Find(bson).SortBy(orderBy).Limit(limit).Skip(skip).ToList();
        }
        public static List<T> GetPagedList2<T, Tk>(string collectionName, int skip, int limit, BsonDocument bson, Expression<Func<T, object>> orderBy)
        {
            var collection = database.GetCollection<T>(collectionName);
            return collection.Find(bson).SortByDescending(orderBy).Limit(limit).Skip(skip).ToList();
        }

        public static int GetCount<T>(string collectionName, BsonDocument bson)
        {
            var collection = database.GetCollection<T>(collectionName);
            return (int)collection.CountDocuments(bson);
            //return collection.Find(bson).ToList().Count;
        }
    }
}
