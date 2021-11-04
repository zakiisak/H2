using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace H2_Øvelser.ThreadPool
{
    internal class Ovelse3
    {
        private static void PrintThreadInfo(object callback)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            

            Console.WriteLine("Thread ID " + Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Thread IsAlive " + Thread.CurrentThread.IsAlive);
            Console.WriteLine("Thread IsBackground " + Thread.CurrentThread.IsBackground);
            Console.WriteLine("Thread IsThreadPoolThread " + Thread.CurrentThread.IsThreadPoolThread);
            Console.WriteLine("Thread Priority " + Thread.CurrentThread.Priority.ToString());
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Thread Pool Info: ");
            System.Threading.ThreadPool.QueueUserWorkItem(PrintThreadInfo);
            Thread.Sleep(50);
            Console.WriteLine(Environment.NewLine + "Normal Thread Info: ");
            new Thread(PrintThreadInfo).Start();
        }
    }
}
