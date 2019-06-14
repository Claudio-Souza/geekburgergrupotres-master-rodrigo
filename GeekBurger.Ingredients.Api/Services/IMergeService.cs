using System.Threading.Tasks;
using GeekBurger.Ingredients.DomainModel;
using GeekBurger.Products.Contract;

namespace GeekBurger.Ingredients.Api.Services
{
    public interface IMergeService
    {
        Task MergeProductWithIngredientsAsync(ProductToGet storeProduct);

        Task UpdateProductsMergesAsync(Ingredient ingredient);
    }
}