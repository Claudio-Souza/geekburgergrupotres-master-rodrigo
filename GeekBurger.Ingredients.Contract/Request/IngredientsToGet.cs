using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.Ingredients.Contract.Request
{
    public class IngredientsToGet
    {
        public List<string> Restrictions { get; set; }
        public Guid StoreId { get; set; }

    }
}
