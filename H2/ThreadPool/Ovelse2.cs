using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace H2_Øvelser.ThreadPool
{
    class Ovelse2
    {
        public static void Main(string[] args)
        {
            Stopwatch mywatch = new Stopwatch();
            Console.WriteLine("Thread Pool Execution");

            mywatch.Start();
            ProcessWithThreadPoolMethod();
            mywatch.Stop();

            Console.WriteLine("Time consumed by ProcessWithThreadPoolMethod is : " + mywatch.ElapsedTicks);
            mywatch.Reset();
            Console.WriteLine("Thread Execution");

            mywatch.Start();
            ProcessWithThreadMethod();
            mywatch.Stop();

            Console.WriteLine("Time consumed by ProcessWithThreadMehtod is : " + mywatch.ElapsedTicks);
        }

        static void ProcessWithThreadPoolMethod()
        {
            for (int i = 0; i <= 10; i++)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(Process);
            }
        }

        static void ProcessWithThreadMethod()
        {
            for (int i = 0; i <= 10; i++)
            {
                Thread obj = new Thread(Process);
                obj.Start();
            }
        }
        static void Process(object callback)
        {
            for(int i = 0; i < 100000; i++)
            {
                for(int y = 0; y < 100000; y++)
                {

                }
            }
        }
    }
}
