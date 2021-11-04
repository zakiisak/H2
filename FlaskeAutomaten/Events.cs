using FlaskeAutomaten.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomaten
{
    public class Events
    {
        public delegate void OnConsumed(Beverage ConsumedBeverage);
        public delegate void OnSplit(Beverage SplittedBeverage);
        public delegate void OnProduced(Beverage ProducedBeverage);
    }
}
