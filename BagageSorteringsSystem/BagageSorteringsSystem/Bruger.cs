using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class Bruger
    {
        private static Random random = new Random();    
        private static bool[] Forks = new bool[5];

        public string Name { get; private set; }
        public int LeftFork { get; private set; }
        public int RightFork { get; private set; }

        public Bruger(string Name, int LeftFork, int RightFork)
        {
            this.Name = Name;
            this.LeftFork = LeftFork;
            this.RightFork = RightFork;
        }

        public void Run(object? state)
        {
            while(true)
            {
                Eat();
                Think();
            }
        }

        public void Think()
        {
            Thread.Sleep(2000 + random.Next(3000));
        }

        public void Eat()
        {
            bool eating = false;
            lock (Forks)
            {
                if(Forks[LeftFork] == false && Forks[RightFork] == false)
                {
                    eating = true;
                    Forks[LeftFork] = true;
                    Forks[RightFork] = true;
                }
            }
            if (eating)
            {
                Console.WriteLine(Name + " is eating");
                Thread.Sleep(4000 + random.Next(8000));
                Console.WriteLine(Name + " stopped eating");
                lock (Forks)
                {
                    Forks[LeftFork] = false;
                    Forks[RightFork] = false;
                }
            }
        }

        public static void Run()
        {

            Bruger bruger1 = new Bruger("1", 4, 0);
            Bruger bruger2 = new Bruger("2", 0, 1);
            Bruger bruger3 = new Bruger("3", 1, 2);
            Bruger bruger4 = new Bruger("4", 2, 3);
            Bruger bruger5 = new Bruger("5", 4, 3);

            Thread t1 = new Thread(bruger1.Run);
            Thread t2 = new Thread(bruger2.Run);
            Thread t3 = new Thread(bruger3.Run);
            Thread t4 = new Thread(bruger4.Run);
            Thread t5 = new Thread(bruger5.Run);
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();
        }
    }
}
