using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaskeAutomaten.Items;

namespace FlaskeAutomaten
{
    public class Buffer
    {
        private Queue<Beverage> Beverages;

        public Buffer(int Size)
        {
            this.Beverages = new Queue<Beverage>(Size);
        }

        public Buffer() : this(100) { }

        public void AddBeverage(Beverage Beverage)
        {
            lock(Beverages)
            {
                Beverages.Enqueue(Beverage);
            }
        }

        public bool IsEmpty()
        {
            bool empty = false;
            lock(Beverages)
            {
                empty = Beverages.Count == 0;
            }
            return empty;
        }

        public Beverage GetNextBeverage()
        {
            lock(Beverages)
            {
                if(Beverages.Count > 0)
                {
                    Beverage beverage = Beverages.Dequeue();
                    return beverage;
                }
            }
            return null;
        }
    }
}
