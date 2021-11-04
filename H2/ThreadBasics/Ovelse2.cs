using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace H2_Øvelser.ThreadBasics
{
    internal class Ovelse2
    {
        static void Print()
        {
            for(int i = 0; i < 5; i++)
            {
                Console.WriteLine("C#-Tråding er nemt");
                Thread.Sleep(1000);
            }
        }

        static void PrintMore()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("også med flere tråde");
                Thread.Sleep(1000);
            }
        }

        public static void Main(string[] args)
        {
            new Thread(Print).Start();
            const int extraThreads = 2;
            for(int i = 0; i < extraThreads; i++)
            {
                new Thread(PrintMore).Start();
            }
        }

    }
}
