using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace H2_Øvelser.ThreadBasics
{
    internal class Ovelse3
    {
        static void CheckTemperature()
        {
            int alarmSignalCount = 0;
            Random random = new Random();
            ConsoleColor defaultColor = Console.ForegroundColor;

            while(alarmSignalCount < 3)
            {
                //Calculates a temperature between -20 and 120
                int temperature = random.Next(140) - 20;
                Console.WriteLine("Målt temperatur: " + temperature);
                if(temperature < 0 || temperature > 100)
                {
                    alarmSignalCount++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    
                    string appendage;

                    if (temperature < 0)
                        appendage = "Temperaturen er " + temperature + " mindre end den må være!";
                    else appendage = "Temperaturen er " + (temperature - 100) + " højere end den må være";

                    Console.WriteLine("Alarm! (" + appendage + ")");
                    Console.ForegroundColor = defaultColor;
                }
                Thread.Sleep(2000);
            }
        }

        public static void Main(string[] args)
        {
            Thread thread = new Thread(CheckTemperature);
            thread.Start();
            while(thread.IsAlive)
            {
                Thread.Sleep(10 * 1000);
            }
            Console.WriteLine("Alarm tråd termineret!");
        }
    }
}
