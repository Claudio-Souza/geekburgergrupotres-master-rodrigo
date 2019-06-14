using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.Ingredients.DomainModel
{
    public class LabelImageAddedMessage
    {
        public string ItemName { get; set; }

        public ICollection<string> Ingredients { get; set; }
    }
}
