using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class Plane
    {
        public Flight Flight;

        //Not currently used, mainly just here for the show
        public List<Passenger> Passengers { get; private set; }
        private List<Luggage> Luggage { get; set; }

        public double LuggageWeight { get; private set; }

        public int MaxPassengers { get; private set; }
        public int MaxLuggageWeight { get; private set; }

        public Plane(Flight Flight, int MaxPassengers, int MaxLuggageWeight)
        {
            this.Flight = Flight;
            this.MaxPassengers = MaxPassengers;
            this.MaxLuggageWeight = MaxLuggageWeight;
            this.Passengers = new List<Passenger>();
            this.Luggage = new List<Luggage>();
        }

        //Updates this plane with new data (used for updating flight plans)
        public void UpdateFromPlane(Plane Plane)
        {
            lock(this)
            {
                lock(Flight)
                {
                    Flight = Plane.Flight;
                }
                this.MaxPassengers = Plane.MaxPassengers;
                this.MaxLuggageWeight = Plane.MaxLuggageWeight;
            }
        }

        public bool CanBoardLuggage(Luggage Luggage)
        {
            if(Luggage != null)
            {
                return LuggageWeight + Luggage.Weight <= MaxLuggageWeight;
            }
            return false;
        }

        public int GetLuggageCount()
        {
            lock(this.Luggage)
            {
                return Luggage.Count;
            }
        }

        public void BoardLuggage(Luggage Luggage)
        {
            if(Luggage != null)
            {
                lock(this.Luggage)
                {
                    LuggageWeight += Luggage.Weight;
                    this.Luggage.Add(Luggage);
                }
                
            }
        }

        public bool BoardPassenger(Passenger Passenger)
        {
            if(Passenger != null)
            {
                if(Passengers.Count < MaxPassengers)
                {
                    Passengers.Add(Passenger);
                    return true;
                }
                return false;
            }
            return false;
        }

    }
}
