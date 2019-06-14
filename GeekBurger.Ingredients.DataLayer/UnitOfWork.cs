using System;
using System.Collections.Generic;
using System.Text;
using GeekBurger.Ingredients.DataLayer.Repositories;
using GeekBurger.Ingredients.DomainModel;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GeekBurger.Ingredients.DataLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private IMongoDatabase _mongoDatabase;

        public UnitOfWork(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        private IIngredientRepository _ingredientsRepository;

        public IIngredientRepository IngredientsRepository {
            get
            {
                if (_ingredientsRepository == null)
                {
                    _ingredientsRepository = new IngredientRepository(_mongoDatabase.GetCollection<Ingredient>("Ingredients"));
                }

                return _ingredientsRepository;
            }
        }

        private ILogRepository _logRepository;

        public ILogRepository LogRepository
        {
            get
            {
                if (_logRepository == null)
                {
                    _logRepository = new LogRepository(_mongoDatabase.GetCollection<BsonDocument>("Logs"));
                }

                return _logRepository;
            }
        }

        private IMergedProductsRepository _mergedProductsRepository;

        public IMergedProductsRepository MergedProductsRepository
        {
            get
            {
                if (_mergedProductsRepository == null)
                {
                    _mergedProductsRepository = new MergedProductsRepository(_mongoDatabase.GetCollection<ProductWithIngredients>("ProductWithIngredients"));
                }

                return _mergedProductsRepository;
            }
        }
    }
}
