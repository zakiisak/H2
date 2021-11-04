using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadTests
{
    public class Program
    {
        private static Queue<int> Test = new Queue<int>();
        private static int iterations = 10000000;

        private static void Consume()
        {
            int counter = 0;
            int vainIterations = 0;
            Stopwatch watch = Stopwatch.StartNew();
            while (counter < iterations)
            {
                lock(Test)
                {
                    if (Test.Count > 0)
                        Test.Dequeue();
                    else vainIterations++;
                }
                counter++;
                if (counter % 10000 == 0)
                    Console.WriteLine(counter);
            }
            watch.Stop();
            Console.WriteLine("It took " + watch.ElapsedMilliseconds + " to run consume 1");
            Console.WriteLine("Vain Iterations: " + vainIterations);
            Environment.Exit(0);
        }
        private static void Consume2()
        {
            int counter = 0;
            Stopwatch watch = Stopwatch.StartNew();
            while (counter < iterations)
            {
                lock (Test)
                {
                    while (Test.Count == 0)
                        Monitor.Wait(Test);
                    Test.Dequeue();
                }
                counter++;
                if (counter % 10000 == 0)
                    Console.WriteLine(counter);
            }
            watch.Stop();
            Console.WriteLine("It took " + watch.ElapsedMilliseconds + " to run consume 2");
            Environment.Exit(0);
        }

        private static void Produce()
        {
            int[] randomNums = new int[50000];
            for(int i = 0; i < randomNums.Length; i++)
            {
                randomNums[i] = i;
            }

            while(true)
            {
                var sum = 0;
                foreach (int num in randomNums)
                    sum += num / 2 * 3;
                lock(Test)
                {
                    Test.Enqueue(sum);
                }
            }
        }

        private static void Produce2()
        {
            int[] randomNums = new int[5000];
            for (int i = 0; i < randomNums.Length; i++)
            {
                randomNums[i] = i;
            }

            while (true)
            {
                var sum = 0;
                foreach (int num in randomNums)
                    sum += num;
                lock (Test)
                {

                    Test.Enqueue(sum);
                    Monitor.PulseAll(Test);
                }
            }
        }

        public static void Main(string[] args)
        {
            new Thread(Consume).Start();
            new Thread(Produce).Start();
        }
    }
}
