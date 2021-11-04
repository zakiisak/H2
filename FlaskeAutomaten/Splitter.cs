using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlaskeAutomaten.Items;

namespace FlaskeAutomaten
{
    public class Splitter
    {
        public Buffer Input { get; private set; }

        private Dictionary<Type, Buffer> Output;
        public bool Running { get; private set; }
        public Events.OnSplit OnSplit { get; set; }

        //The amount of time in milliseconds before it splits the next beverage item.
        public int Delay { get; private set; }

        public Splitter(Buffer Input, params KeyValuePair<Type, Buffer>[] Output)
        {
            this.Input = Input;
            this.Output = new Dictionary<Type, Buffer>();
            foreach(KeyValuePair<Type, Buffer> entry in Output)
            {
                this.Output.Add(entry.Key, entry.Value);
            }
            this.Delay = 100;
        }

        public void SplitBeverage(object? state)
        {
            while(Running)
            {
                Beverage nextBeverage = Input.GetNextBeverage();
                if (nextBeverage != null)
                {
                    Type beverageType = nextBeverage.GetType();

                    if (Output.ContainsKey(beverageType))
                    {
                        Buffer buffer = Output[beverageType];
                        buffer.AddBeverage(nextBeverage);

                        if (this.OnSplit != null)
                            this.OnSplit(nextBeverage);
                    }
                }

                Thread.Sleep(Delay);
            }
        }

        public void Start()
        {
            Running = true;
            ThreadPool.QueueUserWorkItem(SplitBeverage);
        }

        public void Stop()
        {
            Running = false;
        }
        
    }
}
