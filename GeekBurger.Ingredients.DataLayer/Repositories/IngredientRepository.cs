using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeekBurger.Ingredients.DomainModel;
using MongoDB.Driver;

namespace GeekBurger.Ingredients.DataLayer.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private IMongoCollection<Ingredient> _mongoCollection;

        public IngredientRepository(IMongoCollection<Ingredient> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public async Task<IList<Ingredient>> GetByNamesAsync(IEnumerable<string> productIngredients)
        {
            var filter = Builders<Ingredient>.Filter.In(i => i.Name, productIngredients);
            return await _mongoCollection.Find(filter).ToListAsync();
        }

        public async Task InsertOrUpdateAsync(Ingredient product)
        {
            var options = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Ingredient>.Filter.Eq(i => i.Name, product.Name);

            await _mongoCollection.ReplaceOneAsync(filter, product, options);
        }
    }
}
