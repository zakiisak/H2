using System;
using System.Threading;

namespace H2_Øvelser.ThreadBasics
{
    internal class Program
    {
        public void WorkThreadFunction()
        {
            for(int i = 0; i < 5; i++)
            {
                Console.WriteLine("Hi! My name is " + Thread.CurrentThread.Name);
            }
        }
    }

    class Ovelse0
    {
        public static void Main(string[] args)
        {
            Program pg = new Program();
            Thread thread1 = new Thread(pg.WorkThreadFunction);
            thread1.Name = "The super Thread";

            Thread thread2 = new Thread(pg.WorkThreadFunction);
            thread2.Name = "The bright thread";

            thread1.Start();
            thread2.Start();

            Console.Read();
        }
    }
}
