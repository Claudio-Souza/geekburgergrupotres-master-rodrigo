using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.Ingredients.DomainModel
{
    public class ProductWithIngredients
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        public string StoreId { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; }
    }
}
