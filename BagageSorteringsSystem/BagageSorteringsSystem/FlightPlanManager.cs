using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class FlightPlanManager
    {
        private static string FilePath = "flightPlans.txt";

        private Airport Airport;
        private FileSystemWatcher? FlightPlanWatcher;

        //We need a reference to the airport in order to update it from here.
        public FlightPlanManager(Airport Airport)
        {
            this.Airport = Airport;
        }

        //Starts listening to changes done on the flight plan text file.
        public void Start()
        { 
            try {
                //Stops existing file watchers
                Stop();

                FlightPlanWatcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
                FlightPlanWatcher.Filter = "*.txt";

                //Indicate what change to listen on
                FlightPlanWatcher.NotifyFilter = NotifyFilters.Attributes
                                             | NotifyFilters.CreationTime
                                             | NotifyFilters.DirectoryName
                                             | NotifyFilters.FileName
                                             | NotifyFilters.LastAccess
                                             | NotifyFilters.LastWrite
                                             | NotifyFilters.Security
                                             | NotifyFilters.Size;

                FlightPlanWatcher.EnableRaisingEvents = true;
                FlightPlanWatcher.Changed += OnFlightChanged;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Couldn't construct a flight plan listener! FilePath: " + FilePath + ". Exception: " + ex);
            }
        }

        //Stops all change listening on the flight plan.
        public void Stop()
        {
            if(FlightPlanWatcher != null)
            {
                FlightPlanWatcher.Dispose();
                FlightPlanWatcher = null;
            }
        }

        public bool SaveFlightPlan(List<Plane> Planes)
        {
            try
            {
                string[] lines = new string[Planes.Count];
                for(int i = 0; i < Planes.Count; i++)
                {
                    Plane plane = Planes[i];
                    lines[i] = $"{plane.Flight.GetDestinationName()},{plane.Flight.DepartureTime.ToString("HH:mm:ss")},{plane.MaxPassengers},{plane.MaxLuggageWeight}";
                }

                File.WriteAllLines(FilePath, lines);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save flight plan! " + ex);
                return false;
            }
        }

        public List<Plane> ReadFlightPlanFromFile()
        {
            try
            {
                IEnumerable<string> lines = File.ReadAllLines(FilePath);
                List<Plane> planes = new List<Plane>();

                //Each line will be formatted in DESTINATION,HH:mm:ss,MaxPasengers,MaxLuggageWeight
                foreach (string line in lines)
                {
                    try
                    {
                        string[] properties = line.Split(',');

                        //Parse the name written on the line as the destination enum. Remove all spaces as well so that the enum can be recognized properly
                        Destination destination = (Destination)Enum.Parse(typeof(Destination), properties[0].Replace(" ", ""));

                        //Parses the hour, minute second date time from the line
                        DateTime timeOfDay = DateTime.ParseExact(properties[1], "HH:mm:ss", CultureInfo.InvariantCulture);

                        //Fully constructs the departure date + time of day
                        DateTime departureTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second);

                        int maxPassengers = Convert.ToInt32(properties[2]);
                        int maxWeight = Convert.ToInt32(properties[3]);

                        //Now that we have constructed the fields required from the line, we can instantiate the flight object and the plane object
                        Flight flight = new Flight(destination, departureTime);
                        Plane plane = new Plane(flight, maxPassengers, maxWeight);
                        planes.Add(plane);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                return planes;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read flight plan! - " + ex);
                return null;
            }
        }

        private void OnFlightChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name.ToLower() == "flightplans.txt")
            {
                Debug.WriteLine("Original flight plans changed!");
                List<Plane> newPlanes = ReadFlightPlanFromFile();

                //If the returned flights were null, it means some exception occurred. Let's skip the updating then.
                if (newPlanes != null)
                {
                    Airport.UpdateFlightsFromPlan(newPlanes);
                }
            }
        }
    }
}
