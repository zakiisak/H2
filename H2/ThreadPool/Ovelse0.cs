using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace H2_Øvelser.ThreadPool
{
    internal class ThreadPoolDemo
    {

        public void task1(object obj)
        {
            for(int i = 0; i < 2; i++)
            {
                Console.WriteLine("Task 1 is being executed");
            }
        }

        public void task2(object obj)
        {
            for(int i = 0; i < 2; i++)
            {
                Console.WriteLine("Task 2 is being executed");
            }
        }

        public static void Main(string[] args)
        {
            ThreadPoolDemo tpd = new ThreadPoolDemo();
            for(int i = 0; i < 2; i++)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(tpd.task1);
                System.Threading.ThreadPool.QueueUserWorkItem(tpd.task2);
            }
            Console.Read();
        }
    }
}
