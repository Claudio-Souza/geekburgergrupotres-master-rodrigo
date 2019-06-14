using GeekBurger.Ingredients.DataLayer.Repositories;
using System;

namespace GeekBurger.Ingredients.DataLayer
{
    public interface IUnitOfWork
    {
        IIngredientRepository IngredientsRepository { get; }

        ILogRepository LogRepository { get; }

        IMergedProductsRepository MergedProductsRepository { get; }
    }
}
