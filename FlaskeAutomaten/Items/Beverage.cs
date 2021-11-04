using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomaten.Items
{
    public abstract class Beverage
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public double Price { get; protected set; }

        public Beverage(int Id, string Name, double Price)
        {
            this.Id = Id;
            this.Name = Name;
            this.Price = Price;
        }
    }
}
