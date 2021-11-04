using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagageSorteringsSystem
{
    public class Counter
    {
        public Destination Destination { get; set; }
        private LuggageCarrier AttachedCarrier { get; set; }

        public bool IsOpen { get; private set; }

        public Counter(Destination Destination, LuggageCarrier LuggageCarrier)
        {
            this.Destination = Destination;
            this.AttachedCarrier = LuggageCarrier;
            IsOpen = true;
        }

        public void CloseCounter()
        {
            IsOpen = false;
        }

        public void OpenCounter()
        {
            IsOpen = true;
        }

        public bool ValidateLuggage(Luggage Luggage)
        { 
            if(Luggage != null)
            {
                return Luggage.Weight <= 22;
            }
            return false;
        }

        public void ReceiveLuggage(Luggage Luggage)
        {
            if(Luggage != null)
            {
                AttachedCarrier.AddLuggage(Luggage);
            }
        }
    }
}
