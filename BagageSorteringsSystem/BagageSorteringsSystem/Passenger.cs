using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class Passenger
    {
        public string Name { get; set; }
        public Flight Flight { get; set; }

        public Passenger(string Name, Flight Flight)
        {
            this.Name = Name;
            this.Flight = Flight;
        }
    }
}
