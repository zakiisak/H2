using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace H2_Øvelser.ThreadBasics
{
    class Ovelse4
    {
        private static char Input = '*';
        static void ReadInput()
        {
            while(true)
            {
                char c = Console.ReadKey().KeyChar;
                Console.ReadLine();
                Input = c;
            }
        }

        static void WriteInput()
        {
            while(true)
            {
                Console.Write(Input);
                Thread.Sleep(250);
            }
        }


        public static void Main(string[] args)
        {
            Thread readerThread = new Thread(ReadInput);
            readerThread.Name = "Reader";
            readerThread.Start();

            Thread outputThread = new Thread(WriteInput);
            outputThread.Name = "Output";
            outputThread.Start();
        }
    }
}
