using System.Collections.Generic;
using System.Threading.Tasks;
using GeekBurger.Products.Contract;

namespace GeekBurger.Ingredients.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductToGet>> GetStoreProducts(string storeName);
    }
}