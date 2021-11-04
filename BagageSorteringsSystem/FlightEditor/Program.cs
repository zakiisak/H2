using BagageSorteringsSystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightEditor
{
    public class Program
    {

        //Displays all the flights registered in the flight plan file
        private static void DisplayFlights(FlightPlanManager planManager)
        {
            List<Plane> planes = planManager.ReadFlightPlanFromFile();
            Console.WriteLine("---------- Flight Plan: ----------");
            Console.WriteLine("Country\t\tDeparture\tMaxPassengers\tMaxCarriage");
            foreach (Plane plane in planes)
            {
                Console.WriteLine($"{plane.Flight.GetDestinationName()}\t{(plane.Flight.GetDestinationName().Length > 7 ? "" : "\t")}{plane.Flight.DepartureTime.ToString("HH:mm:ss")}\t{plane.MaxPassengers}\t\t{plane.MaxLuggageWeight}");
            }
            Console.WriteLine();
        }

        //Takes care of adding new flights from console input
        private static void AddNewFlight(FlightPlanManager planManager)
        {
            Destination? destination = null;

            while (destination == null)
            {
                Console.WriteLine("Enter one of the following destinations: ");
                string[] destinationNames = Enum.GetNames(typeof(Destination));
                for (int i = 0; i < destinationNames.Length; i++)
                    Console.WriteLine(i + ": " + destinationNames[i]);
                int index = 0;
                if (int.TryParse(Console.ReadLine(), out index))
                {
                    if (index >= destinationNames.Length || index < 0)
                        Console.WriteLine("Index was out of bounds!");
                    else
                        destination = (Destination)Enum.Parse(typeof(Destination), destinationNames[index]);
                }
            }


            DateTime? outDate = null;
            while (outDate == null)
            {
                Console.WriteLine("Enter the departure time in the format HH:mm:ss : ");
                string timeString = Console.ReadLine();
                DateTime time;
                if (DateTime.TryParseExact(timeString, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                {
                    outDate = time;
                }
                else Console.WriteLine("Wrong format!");
            }

            int maxPassengers = 0;
            do
            {
                Console.WriteLine("Enter the max passenger count:");
            }
            while (int.TryParse(Console.ReadLine(), out maxPassengers) == false);

            int maxWeight = 0;
            do
            {
                Console.WriteLine("Enter the max luggage weight:");
            }
            while (int.TryParse(Console.ReadLine(), out maxWeight) == false);

            Plane plane = new Plane(new Flight((Destination)destination, (DateTime)outDate), maxPassengers, maxWeight);

            List<Plane> planes = planManager.ReadFlightPlanFromFile();
            planes.Add(plane);
            planManager.SaveFlightPlan(planes);
        }

        //Takes care of deleting flights from console input
        private static void DeleteFlight(FlightPlanManager planManager)
        {
            List<Plane> planes = planManager.ReadFlightPlanFromFile();
            Console.WriteLine("---------- Flights: ----------");
            Console.WriteLine("Id\tCountry\t\tDeparture\tMaxPassengers\tMaxCarriage");
            for(int i = 0; i < planes.Count; i++)
            {
                Plane plane = planes[i];
                Console.WriteLine($"{i}\t{plane.Flight.GetDestinationName()}\t{(plane.Flight.GetDestinationName().Length > 7 ? "" : "\t")}{plane.Flight.DepartureTime.ToString("HH:mm:ss")}\t{plane.MaxPassengers}\t\t{plane.MaxLuggageWeight}");
            }

            Console.Write(Environment.NewLine + "Enter the id of the flight you wish to delete");
            string idString = Console.ReadLine();

            int index = 0;
            if(int.TryParse(idString, out index) && index >= 0 && index < planes.Count)
            {
                planes.RemoveAt(index);
                planManager.SaveFlightPlan(planes);
            }
            else
            {
                Console.WriteLine("Invalid number!");
                DeleteFlight(planManager);
            }

            Console.WriteLine();
        }

        public static void Main(string[] args)
        {
            FlightPlanManager planManager = new FlightPlanManager(null);

            Console.WriteLine("Welcome to the flight editor!");
            while(true)
            {
                Console.WriteLine("You can now do 3 things:");
                Console.WriteLine("1: Display the saved flight plan");
                Console.WriteLine("2: Add a new flight to the plan");
                Console.WriteLine("3: Delete a flight from the plan");
                int num = 0;
                if (int.TryParse(Console.ReadLine(), out num))
                {
                    if(num == 1)
                    {
                        DisplayFlights(planManager);
                    }
                    else if(num == 2)
                    {
                        AddNewFlight(planManager);

                    }
                    else if(num == 3)
                    {
                        DeleteFlight(planManager);
                    }
                }
                else Console.WriteLine("Invalid number! Try again.");
            }
        }
    }
}
