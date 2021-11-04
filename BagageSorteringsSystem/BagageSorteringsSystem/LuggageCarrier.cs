using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class LuggageCarrier 
    {
        public bool Rolling { get; private set; }
        private Queue<Luggage> Luggage;

        private LuggageSorter LuggageSorter;
        public List<Luggage> LostLuggage { get; private set; }

        public LuggageCarrier(LuggageSorter LuggageSorter)
        {
            this.Luggage = new Queue<Luggage>();
            this.LostLuggage = new List<Luggage>();
            this.LuggageSorter = LuggageSorter;
        }

        private void Roll(object? state)
        {
            while(Rolling)
            {
                lock(this.Luggage)
                {
                    if (this.Luggage.Count > 0)
                    {
                        Luggage luggage = this.Luggage.Dequeue();
                        if(this.LuggageSorter.BoardLuggage(luggage) == false)
                        {
                            Debug.WriteLine("Bagage til " + luggage.Flight.GetDestinationName() + " kunne ikke boardes da flyet allerede er taget afsted");
                            LostLuggage.Add(luggage);
                        }
                    }
                }
                //Simulate transportation time
                Thread.Sleep(1000);
            }
        }

        public void AddLuggage(Luggage Luggage)
        {
            lock(this.Luggage)
            {
                this.Luggage.Enqueue(Luggage);
            }
        }

        public bool CanBoardLuggage(Luggage Luggage)
        {
            if(Luggage != null)
            {
                if (Luggage.Weight <= 22)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetLuggageCount()
        {
            return Luggage.Count;
        }

        public void BeginRolling()
        {
            Rolling = true;
            ThreadPool.QueueUserWorkItem(Roll);
        }

        public void StopRolling()
        {
            Rolling = false;
        }
        
    }
}
