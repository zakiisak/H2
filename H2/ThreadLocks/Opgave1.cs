using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace H2_Øvelser.ThreadLocks
{
    internal class Opgave1
    {
        int num = 0;

        void ThreadCountUp(object obj)
        {
            while(true)
            {
                //Count up
                Thread.Sleep(2000);
            }
        }

        void ThreadCountDown(object? state)
        {
            while(true)
            {
                //Count down
            }
        }

        public static void Main(string[] args)
        {
            object obj = new object();
            Opgave1 objekt = new Opgave1();
            
            //System.Threading.ThreadPool.QueueUserWorkItem(ThreadCountUp);
            //System.Threading.ThreadPool.QueueUserWorkItem(ThreadCountDown);
        }
    }
}
