using GeekBurger.Ingredients.DomainModel;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.DataLayer.Repositories
{
    public class MergedProductsRepository : IMergedProductsRepository
    {
        private IMongoCollection<ProductWithIngredients> _mongoCollection;

        public MergedProductsRepository(IMongoCollection<ProductWithIngredients> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public async Task DeleteAsync(Guid productId)
        {
            var filter = Builders<ProductWithIngredients>.Filter.Eq(p => p.Id, productId.ToString());
            await _mongoCollection.DeleteOneAsync(filter);
        }

        public IEnumerable<ProductWithIngredients> GetAll()
        {
            foreach (var product in _mongoCollection.Find(new BsonDocument()).ToEnumerable())
            {
                yield return product;
            }
        }

        public async Task<IEnumerable<ProductWithIngredients>> GetProductRestrictionByStore(Guid storeId, List<string> restrictions)
        {
            var storeFilter = Builders<ProductWithIngredients>.Filter.Eq(p => p.StoreId, storeId.ToString());
            var restrictionFilter = Builders<ProductWithIngredients>.Filter
                .In("Ingredients.Composition", restrictions);

            var filter = storeFilter & restrictionFilter;

            return await _mongoCollection.Find(filter).ToListAsync();
        }

        public async Task<ProductWithIngredients> InsertOrUpdate(ProductWithIngredients productWithIngredients)
        {
            var filter = Builders<ProductWithIngredients>.Filter.Eq(p => p.Id, productWithIngredients.Id);
            var options = new UpdateOptions { IsUpsert = true };

            await _mongoCollection.ReplaceOneAsync(filter, productWithIngredients, options);

            return productWithIngredients;
        }
    }
}