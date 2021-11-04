using FlaskeAutomaten.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlaskeAutomaten
{
    public class Consumer
    {
        private Buffer Input;
        public bool Running { get; private set; }
        public Type BufferType { get; private set; }
        public Events.OnConsumed OnConsumed { get; set; }
        public int ConsumeDelay { get; private set; }

        public Consumer(Buffer Input, Type BufferType)
        {
            this.Input = Input;
            this.BufferType = BufferType;
            ConsumeDelay = 1200;
        }

        private void Consume(object? state)
        {
            while(Running)
            {
                if (Input.IsEmpty() == false)
                {
                    Thread.Sleep(ConsumeDelay);
                    //This funciton retrieves and removes it from the containing queue.
                    //We don't need to do anything further from this point
                    Beverage consumedBeverage = Input.GetNextBeverage();

                    if (this.OnConsumed != null)
                        this.OnConsumed(consumedBeverage);
                }
                else Thread.Sleep(2);

            }
        }

        public void Start()
        {
            Running = true;
            ThreadPool.QueueUserWorkItem(Consume);
        }

        public void Stop()
        {
            Running = false;
        }
    }
}
