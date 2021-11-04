using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class Airport
    {
        public Counter[] Counters { get; private set; }
        public LuggageCarrier LuggageCarrier { get; private set; }
        public LuggageSorter LuggageSorter { get; private set; }
        public List<Terminal> Terminals { get; private set; }
        private FlightPlanManager FlightPlanManager { get; set; }

        public bool ServicesRunning { get; private set; }

        private Random random;

        public Airport()
        {
            this.LuggageSorter = new LuggageSorter(this);  
            this.LuggageCarrier = new LuggageCarrier(LuggageSorter);
            
            //Construct counters for all possible destinations, and open/close them later based on if there are flights to the given destination.
            Array allDestinations = Enum.GetValues(typeof(Destination));
            this.Counters = new Counter[allDestinations.Length];

            for(int i = 0; i < this.Counters.Length; i++)
            {
                this.Counters[i] = new Counter((Destination) allDestinations.GetValue(i), this.LuggageCarrier);
            }
            this.Terminals = new List<Terminal>();
            this.FlightPlanManager = new FlightPlanManager(this);
            this.random = new Random();
            AddSimulationData();
        }

        #region Services

        //A service that takes care of departing planes (which is to say remove them from the terminals lists), when the luggage weight
        //exceeds the allowed amount, or the departure time is due.
        private void ControlDepartingPlanes(object? state)
        {
            while(ServicesRunning)
            {
                foreach (Terminal terminal in Terminals)
                {
                    Plane? planeToDepart = null;

                    //Lock it while we iterate through it
                    lock (terminal.Planes)
                    {
                        foreach (Plane plane in terminal.Planes)
                        {
                            //If the weight exeeds the max luggage weight, or the departure time is due,
                            //depart the plane
                            if (plane.LuggageWeight >= (double)plane.MaxLuggageWeight || DateTime.Now >= plane.Flight.DepartureTime)
                            {
                                planeToDepart = plane;
                            }
                        }
                        if(planeToDepart != null)
                        {
                            terminal.DepartPlane(planeToDepart.Flight);
                            Debug.WriteLine("Plane to " + planeToDepart.Flight.GetDestinationName() + " now departing");
                        }
                    }
                }
                Thread.Sleep(1500);
            }
        }

        //A service that takes care of checking in random luggage, that can weigh between 0 and 70 kg
        private void AddRandomLuggage(object? state)
        {
            List<Flight> possibleFlights = GetAllFlights();
            
            while(ServicesRunning)
            {
                Flight flight = possibleFlights[random.Next(possibleFlights.Count)];
                Luggage luggage = new Luggage(flight, random.NextDouble() * 70);
                CheckinLuggage(luggage);
                Thread.Sleep(250 + random.Next(2000));
            }
        }

        //A service that takes care of adding random new planes to the airport. Adds them to a random terminal.
        private void HandleIncomingPlanes(object? state)
        {
            while(ServicesRunning)
            {
                List<Plane> currentPlanes = GetAllPlanes();

                Array allDestinations = Enum.GetValues(typeof(Destination));

                foreach(Destination destination in allDestinations)
                {
                    //If a destination is not present, it should try to construct a new flight within a few seconds with that plane
                    if(currentPlanes.Find(plane => plane.Flight.Destination == destination ) == null)
                    {
                        Plane plane = new Plane(new Flight(destination, DateTime.Now.AddSeconds(30 + random.Next(40))), 10 + random.Next(50), 50 + random.Next(120));
                        int randomTerminalIndex = random.Next(Terminals.Count);
                        Terminal terminal = Terminals[randomTerminalIndex];
                        lock(terminal.Planes)
                        {
                            terminal.Planes.Add(plane);
                        }
                    }
                }

                //Every 10 seconds, fill out the empty spaces with newly generated planes
                Thread.Sleep(10000);
            }
        }

        //Iterates through the counters every 3 seconds, and closes/opens them based on if the corresponding planes have left the airport
        private void HandleCounters(object? state)
        {
            while(ServicesRunning)
            {
                List<Flight> flights = GetAllFlights();
                lock(Counters)
                {
                    for(int i = 0; i < Counters.Length; i++)
                    {
                        Counter counter = Counters[i];
                        bool stillAvailable = false;
                        foreach(Flight flight in flights)
                        {
                            if(flight.Destination.Equals(counter.Destination))
                            {
                                stillAvailable = true;
                                break;
                            }
                        }

                        if (stillAvailable == false)
                        {
                            counter.CloseCounter();
                        }
                        else if (counter.IsOpen == false)
                            counter.OpenCounter();
                    }
                }
                Thread.Sleep(3000);
            }
        }

        #endregion

        //Starts all the worker threads that operate the airport.
        public void StartServices()
        {
            this.ServicesRunning = true;
            this.LuggageCarrier.BeginRolling();
            this.FlightPlanManager.Start();
            ThreadPool.QueueUserWorkItem(AddRandomLuggage);
            ThreadPool.QueueUserWorkItem(ControlDepartingPlanes);
            ThreadPool.QueueUserWorkItem(HandleIncomingPlanes);
        }


        //Stop the worker threads
        public void StopServices()
        {
            this.ServicesRunning = false;
            this.LuggageCarrier.StopRolling();
            this.FlightPlanManager.Stop();
        }

        //The main function to check in luggage
        public StatusCode CheckinLuggage(Luggage Luggage)
        {
            lock (Counters)
            {
                foreach (Counter counter in Counters)
                {
                    if (counter.Destination.Equals(Luggage.Flight.Destination))
                    {
                        //Validate whether the luggage is too heavy or not
                        if (counter.ValidateLuggage(Luggage))
                        {
                            //Send the luggage on the luggage carrier through the connected counter.
                            counter.ReceiveLuggage(Luggage);
                            return StatusCode.Success;
                        }
                        return StatusCode.TooHeavy;
                    }
                }
            }
            return StatusCode.NotFound;
        }

        //The main function to update the flights from the new flight plan in the text file.
        public void UpdateFlightsFromPlan(List<Plane> flightPlan)
        {
            //Update the 
            List<Plane> existingPlanes = GetAllPlanes();
            foreach(Plane plane in flightPlan)
            {
                Plane existingPlane = existingPlanes.Find(pl => pl.Flight.Destination.Equals(plane.Flight.Destination));
                if(existingPlane != null)
                {
                    //Lock the flight object, so that we don't 
                    existingPlane.UpdateFromPlane(plane);
                }
                //If the existingFlight is null, it means it doesn't exist, which means we should create it!
                else
                {
                    //Choose a random terminal
                    Terminal terminal = Terminals[random.Next(Terminals.Count)];

                    //Add the new plane to the terminal
                    lock(terminal.Planes)
                    {
                        terminal.Planes.Add(plane);
                    }
                }
            }
        }


        #region Getter Functions

        public List<Flight> GetAllFlights()
        {

            List<Flight> flights = new List<Flight>();
            lock (Terminals)
            {
                foreach (Terminal terminal in Terminals)
                {
                    lock (terminal)
                    {
                        foreach (Plane plane in terminal.Planes)
                        {
                            flights.Add(plane.Flight);
                        }
                    }
                }
            }
            return flights;
        }

        public List<Plane> GetAllPlanes()
        {
            List<Plane> planes = new List<Plane>();
            foreach (Terminal terminal in Terminals)
            {
                lock(terminal)
                {
                    planes.AddRange(terminal.Planes);
                }
            }
            return planes;
        }

        public Plane GetPlaneFromFlight(Flight Flight)
        {
            foreach (Terminal terminal in Terminals)
            {
                foreach (Plane plane in terminal.Planes)
                {
                    if (plane.Flight.Equals(Flight))
                    {
                        return plane;
                    }
                }
            }
            return null;
        }

        #endregion

        //Adds test data to the 
        private void AddSimulationData()
        {
            Flight londonFlight = new Flight(Destination.London, DateTime.Now.AddSeconds(30));
            Flight parisFlight = new Flight(Destination.Paris, DateTime.Now.AddSeconds(15));
            Flight brusselsFlight = new Flight(Destination.Brussels, DateTime.Now.AddSeconds(60));
            Flight edinburghFlight = new Flight(Destination.Edinburgh, DateTime.Now.AddSeconds(40));

            List<Plane> Planes = new List<Plane>();

            Terminal terminal1 = new Terminal(1, new Plane(londonFlight, 80, 300), new Plane(parisFlight, 80, 30));
            Terminal terminal2 = new Terminal(2, new Plane(brusselsFlight, 80, 8000));
            Terminal terminal3 = new Terminal(3, new Plane(edinburghFlight, 80, 12000));

            Terminals.Add(terminal1);
            Terminals.Add(terminal2);
            Terminals.Add(terminal3);
        }
    }
}
