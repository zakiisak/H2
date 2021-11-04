using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class Terminal
    {
        public int TerminalNumber { get; private set; }
        public List<Plane> Planes { get; private set; }

        public Terminal(int TerminalNumber, params Plane[] Planes)
        {
            this.TerminalNumber = TerminalNumber;
            this.Planes = new List<Plane>();
            this.Planes.AddRange(Planes);
        }

        //Removes the first found plane in the terminal containing the given flight
        public void DepartPlane(Flight Flight)
        {
            lock(Planes)
            {
                for(int i = 0; i < Planes.Count; i++)
                {
                    if(Planes[i].Flight.Equals(Flight))
                    {
                        Planes.RemoveAt(i);
                        break;
                    }
                }

            }
        }
    }
}
