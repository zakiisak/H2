using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class Luggage
    {
        public Flight Flight { get; private set; }

        //The weight of this luggage, in kilograms
        public double Weight { get; set; }

        public Luggage(Flight Flight, double Weight)
        {
            this.Flight = Flight;
            this.Weight = Weight;
        }

    }
}
