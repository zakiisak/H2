using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class Flight
    {
        public Destination Destination { get; private set; }
        public DateTime DepartureTime { get; private set; }

        public Flight(Destination Destination, DateTime DepartureTime)
        {
            this.Destination = Destination;
            this.DepartureTime = DepartureTime;
        }

        //Updates the departure time from an external source
        public void UpdateDepartureTime(DateTime NewDeparture)
        {
            DepartureTime = NewDeparture;
        }

        //Returns the name string of the enum destination
        public string GetDestinationName()
        {
            string? name = Enum.GetName(typeof(Destination), this.Destination);
            if (name != null)
                return name;
            return "null";
        }

        //Used to uniquely identifying flight objects in lists etc.
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if(obj is Flight)
            {
                Flight flight = obj as Flight;
                return this.Destination.Equals(flight.Destination);

                //The below was commented out due to the complexity it would cause if the flight plan was changed while the program was running
                //Now we just go by the destination
                //return this.Destination == flight.Destination && this.DepartureTime == flight.DepartureTime;
            }
            return false;
        }
    }
}
