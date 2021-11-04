using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class LuggageSorter
    {
        public Airport Airport;
        public LuggageSorter(Airport airport)
        {
            this.Airport = airport;
        }

        public bool BoardLuggage(Luggage Luggage)
        {
            //Find the plane that this luggage belongs to
            Plane attachedPlane = Airport.GetPlaneFromFlight(Luggage.Flight);

            //If no plane was found, it will be null
            if (attachedPlane != null)
            {
                //Board the luggage on the plane.
                attachedPlane.BoardLuggage(Luggage);
                return true;
            }
            return false;
        }
    }
}
