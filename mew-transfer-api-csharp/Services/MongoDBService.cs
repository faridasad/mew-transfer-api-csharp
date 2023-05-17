using mew_transfer_api_csharp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mew_transfer_api_csharp.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Record> _recordCollection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _recordCollection = database.GetCollection<Record>(mongoDBSettings.Value.CollectionName);
        }

        public async Task<List<Record>> GetAsync()
        {
            return await _recordCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task Add(Record record)
        {
            await _recordCollection.InsertOneAsync(record);
        }
    }
}
