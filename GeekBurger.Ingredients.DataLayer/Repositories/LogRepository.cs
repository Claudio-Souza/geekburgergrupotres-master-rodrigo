using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GeekBurger.Ingredients.DataLayer.Repositories
{
    public class LogRepository : ILogRepository
    {
        private IMongoCollection<BsonDocument> _mongoCollection;

        public LogRepository(IMongoCollection<BsonDocument> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public async Task SaveAsync(string logMessage)
        {
            await _mongoCollection.InsertOneAsync(new BsonDocument { { "Message", logMessage } });
        }
    }
}