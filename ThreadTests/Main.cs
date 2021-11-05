using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadTests
{
    public class main
    {
        private static Bakery bakery;

        private static void Producer()
        {
            Donut[] freshDonuts = new Donut[4];
            for (int i = 0; i < freshDonuts.Length; i++)
                freshDonuts[i] = new Donut(i);
            while(true)
            {
                bakery.RefillTray(freshDonuts);
                Console.WriteLine("Produced " + freshDonuts.Length + " donuts");
            }
        }

        private static void Consumer()
        {
            while(true)
            {
                Donut donut = bakery.GetDonut();
                Console.WriteLine("Consumed donut " + donut.Id);
            }
        }


        public static void Main(string[] args)
        {
            bakery = new Bakery();

            Thread t1 = new Thread(Producer);
            t1.Start();
            Thread t2 = new Thread(Consumer);
            t2.Start();
        
        }
    }
}
