using GeekBurger.Ingredients.DomainModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.DataLayer.Repositories
{
    public interface IMergedProductsRepository
    {
        Task<ProductWithIngredients> InsertOrUpdate(ProductWithIngredients productWithIngredients);

        Task DeleteAsync(Guid productId);

        Task<IEnumerable<ProductWithIngredients>> GetProductRestrictionByStore(Guid storeId, List<string> restrictions);
        
        IEnumerable<ProductWithIngredients> GetAll();
    }
}