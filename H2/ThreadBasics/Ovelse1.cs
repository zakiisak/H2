using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace H2_Øvelser.ThreadBasics
{
    internal class Ovelse1
    {
        static void Print()
        {
            for (int i = 0; i < 5; i++)
                Console.WriteLine("C#-Tråding er nemt!");
        }

        public static void Main(string[] args)
        {
            new Thread(Print).Start();
        }
    }
}
