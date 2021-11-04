using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            Airport airport = new Airport();
            airport.StartServices();
            Console.WriteLine($"Velkommen til lufthavnen! ");
            
            while (true)
            {
                Console.WriteLine(Environment.NewLine + "Du har nu følgende muligheder:");
                Console.WriteLine("1 - Check din baggage ind");
                Console.WriteLine("2 - Vis antallet af bagage på bæltet");
                Console.WriteLine("3 - Vis over sigt over fly og deres optaget vægt");
                Console.WriteLine("4 - Vis Antal af mistet bagage");
                Console.Write("5 - Start/Stop Lufthavnens enheder. (Nuværdige status: ");
                Console.ForegroundColor = airport.ServicesRunning ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine((airport.ServicesRunning ? "Kører" : "Nede") + ")");
                Console.ForegroundColor = ConsoleColor.Gray;

                char num = Console.ReadKey().KeyChar;
                Console.Clear();

                if (num == '1')
                {
                    Console.WriteLine("Mulige destinationer: ");

                    List<Flight> flights = airport.GetAllFlights();
                    for (int i = 0; i < flights.Count; i++)
                    {
                        Flight flight = flights[i];
                        Console.WriteLine($"[{i}]: " + flight.GetDestinationName() + " @ " + flight.DepartureTime.ToString("HH:mm:ss"));
                    }
                    Console.WriteLine();

                    Console.WriteLine("Indtast din destination:");
                    char destIndex = Console.ReadKey().KeyChar;
                    Console.WriteLine();

                    Console.WriteLine(Environment.NewLine + "Indtast din bagages vægt i kilogram");
                    string weightString = Console.ReadLine();

                    double weight = 0;
                    if(double.TryParse(weightString, out weight))
                    {
                        Luggage luggage = new Luggage(flights[Convert.ToInt32("" + destIndex)], weight);

                        StatusCode code = airport.CheckinLuggage(luggage);

                        if(code == StatusCode.Success)
                        {
                            airport.LuggageCarrier.AddLuggage(luggage);
                            Console.WriteLine("Luggage boarded!");
                        }
                        else if(code == StatusCode.TooHeavy)
                        {
                            Console.WriteLine("Luggage weighs over 22 kg! Not boarding");
                        }
                        else if(code == StatusCode.NotFound)
                        {
                            Console.WriteLine("The counter is not present anymore!");
                        }
                    }
                }
                else if(num == '2')
                {
                    Console.WriteLine("Luggage Count: " + airport.LuggageCarrier.GetLuggageCount());
                }
                else if(num == '3')
                {
                    List<Plane> planes = airport.GetAllPlanes();
                    foreach(Plane plane in planes)
                    {
                        string destinationName = plane.Flight.GetDestinationName();
                        Console.WriteLine("----------- " + destinationName + " -----------");
                        Console.WriteLine("Afgang: " + plane.Flight.DepartureTime.ToString("HH:mm:ss"));
                        Console.WriteLine("Antal af bagage: " + plane.GetLuggageCount());
                        Console.WriteLine("Bagage Vægt: " + plane.LuggageWeight + " / " + plane.MaxLuggageWeight + " (kg)");
                        Console.WriteLine();
                    }
                }
                else if(num == '4')
                {
                    Console.WriteLine("Antal af mistet bagage: " + airport.LuggageCarrier.LostLuggage.Count);
                }
                else if(num == '5')
                {
                    if (airport.ServicesRunning)
                        airport.StopServices();
                    else airport.StartServices();
                }
            }
        }

    }
}
