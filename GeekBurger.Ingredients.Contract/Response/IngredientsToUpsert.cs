using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.Ingredients.Contract.Response
{
   public class IngredientsToUpsert
    {
        public List<string> Ingredients { get; set; }
        public Guid ProductId { get; set; } 
    }
}
