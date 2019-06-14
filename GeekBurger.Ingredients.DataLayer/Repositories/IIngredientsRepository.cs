using System.Collections.Generic;
using System.Threading.Tasks;
using GeekBurger.Ingredients.DomainModel;

namespace GeekBurger.Ingredients.DataLayer.Repositories
{
    public interface IIngredientRepository
    {
        Task InsertOrUpdateAsync(Ingredient product);

        Task<IList<Ingredient>> GetByNamesAsync(IEnumerable<string> productIngredients);
    }
}