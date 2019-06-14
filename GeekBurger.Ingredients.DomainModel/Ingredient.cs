using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GeekBurger.Ingredients.DomainModel
{
    public class Ingredient
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<string> Composition { get; set; }
    }
}
