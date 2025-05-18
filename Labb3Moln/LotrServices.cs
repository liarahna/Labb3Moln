using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3Moln
{
    public class LotrServices
    {
        private readonly IMongoCollection<Lotr> _lotrCollection;

        public LotrServices(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _lotrCollection = database.GetCollection<Lotr>("lotr");
        }

        public async Task<List<Lotr>> GetLotrAsync() =>
            await _lotrCollection.Find(_ => true).ToListAsync();

        public async Task<Lotr> GetByIdAsync(string id) =>
            await _lotrCollection.Find(l => l.Id == id).FirstOrDefaultAsync();

        public async Task CreateLotrAsync(Lotr lotr) =>
            await _lotrCollection.InsertOneAsync(lotr);
    }
}
