using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaskeAutomaten.Items;
using System.Diagnostics;
using System.Threading;

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
                Monitor.PulseAll(Beverages);
            }
        }

        public void Lock()
        {
            lock (Beverages)
            {
                while (true)
                {

                }
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
                while(Beverages.Count == 0)
                {
                    Monitor.Wait(Beverages);
                }

                Beverage beverage = Beverages.Dequeue();
                return beverage;
            }
        }
    }
}