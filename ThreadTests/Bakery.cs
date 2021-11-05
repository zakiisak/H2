using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadTests
{
    public class Bakery
    {
        Queue<Donut> _donutTray = new Queue<Donut>();

        public Donut GetDonut()
        {
            lock(_donutTray)
            {
                while(_donutTray.Count == 0)
                {
                    Monitor.Wait(_donutTray);
                }

                return _donutTray.Dequeue();
            }
        }

        public void RefillTray(Donut[] freshDonuts)
        {
            lock(_donutTray)
            {
                foreach(Donut donut in freshDonuts)
                {
                    _donutTray.Enqueue(donut);
                }
                Monitor.PulseAll(_donutTray);
            }
        }
    }
}
