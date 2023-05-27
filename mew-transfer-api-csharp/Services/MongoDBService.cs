using mew_transfer_api_csharp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace mew_transfer_api_csharp.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Record> _recordCollection;
        private readonly IMongoDatabase _database;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            _database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _recordCollection = _database.GetCollection<Record>(mongoDBSettings.Value.CollectionName);
        }

        public async Task<List<Record>> GetAllAsync()
        {
            return await _recordCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Record> GetAsync(string id)
        {
            return await _recordCollection.Find(record => record.Id == new ObjectId(id)).FirstOrDefaultAsync();
        }

        public async Task Add(Record record)
        {
            await _recordCollection.InsertOneAsync(record);
        }

        public async Task<ObjectId> UploadFileAsync(string fileName, Stream fileStream)
        {
            var bucket = new GridFSBucket(_database);
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument("FileName", fileName)
            };

            return await bucket.UploadFromStreamAsync(fileName, fileStream, options);
        }
    }
}
