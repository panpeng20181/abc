using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace preview
{
    public class MongoDBOperation<T> where T : class
    {
        private static MongoDBOperation<T> mongoDBOperation = null;
        private static readonly object lockobject = new object();
        private MongoClient mongoClient { get; set; }
        private IMongoDatabase db { get; set; }
        private IMongoCollection<BsonDocument> collection { get; set; }

        private IEnumerable<BsonDocument> documents { get; set; }

        private MongoDBOperation()
        {
            mongoClient = new MongoClient("mongodb://localhost:27017");
            db = mongoClient.GetDatabase("db");
            collection = db.GetCollection<BsonDocument>("file");
        }
        public static MongoDBOperation<T> GetMongoDBInstance()
        {
            if (mongoDBOperation == null)
            {
                lock (nameof(MongoDBOperation<T>))// lockobject)
                {
                    if (mongoDBOperation == null)
                    {
                        mongoDBOperation = new MongoDBOperation<T>();
                    }
                }
            }

            return mongoDBOperation;
        }

        /// <summary>
        /// 同步插入数据
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool InsertOneData(BsonDocument document)
        {
            try
            {
                if (collection != null)
                {
                    collection.InsertOne(document);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 异步插入
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsyncOneData(BsonDocument document)
        {
            try
            {
                if (collection != null)
                {
                    await collection.InsertOneAsync(document);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 同步插入多条数据
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        public bool InsertManyData(IEnumerable<BsonDocument> documents)
        {
            try
            {
                //documents = Enumerable.Range(0, 100).Select(i => new BsonDocument("counter", i));
                if (collection != null)
                {
                    collection.InsertMany(documents);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 同步插入多条数据
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsyncManyData(IEnumerable<BsonDocument> documents)
        {
            try
            {
                //documents = Enumerable.Range(0, 100).Select(i => new BsonDocument("counter", i));
                if (collection != null)
                {
                    await collection.InsertManyAsync(documents);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 查找有数据。
        /// </summary>
        /// <returns></returns>
        public List<BsonDocument> FindData()
        {
            return collection.Find(new BsonDocument()).ToList();
        }

        /// <summary>
        /// 取排除_id字段以外的数据。然后转换成泛型。
        /// </summary>
        /// <returns></returns>
        public List<BsonDocument> FindAnsyncData()
        {
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var document = collection.Find(new BsonDocument()).Project(projection).ToListAsync().Result;
            return document;
        }

        /// <summary>
        /// 按某些列条件查询
        /// </summary>
        /// <param name="bson"></param>
        /// <returns></returns>
        public List<BsonDocument> FindFilterlData(BsonDocument bson)
        {
            var buildfilter = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = null;

            foreach (var bs in bson)
            {
                filter = buildfilter.Eq(bs.Name, bs.Value);
            }
            //filter = buildfilter.Eq("name", "MongoDBTest");
            var documents = collection.Find(filter).ToList();
            return documents;
        }


        /// <summary>
        /// 返回受影响行
        /// </summary>
        /// <returns></returns>
        public long DeleteData()
        {
            //删除count大于0的文档。
            var filter = Builders<BsonDocument>.Filter.Gt("count", 0);
            DeleteResult deleteResult = collection.DeleteMany(filter);
            return deleteResult.DeletedCount;
        }

        /// <summary>
        /// 根据id更新文档中单条数据。
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="bson"></param>
        public UpdateResult UpdateOneData(string _id, BsonDocument bson)
        {
            //修改条件（相当于sql where）
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("name", "MongoDB");
            UpdateDefinition<BsonDocument> update = null;
            foreach (var bs in bson)
            {
                if (bs.Name.Equals("name"))
                {
                    update = Builders<BsonDocument>.Update.Set(bs.Name, bs.Value);
                }
            }
            //UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("name", bson[0].ToString());
            UpdateResult result = collection.UpdateOne(filter, update);//默认更新第一条。
            return result;
        }
    }
}