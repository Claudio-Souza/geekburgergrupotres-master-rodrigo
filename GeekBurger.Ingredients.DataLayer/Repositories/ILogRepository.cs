using System.Threading.Tasks;

namespace GeekBurger.Ingredients.DataLayer.Repositories
{
    public interface ILogRepository
    {
        Task SaveAsync(string logMessage);
    }
}