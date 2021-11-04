using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlaskeAutomaten.Items;

namespace FlaskeAutomaten
{
    public class Producer
    {
        public bool Running { get; private set; }
        public Buffer Buffer { get; private set; }
        public Events.OnProduced OnProduced { get; set;}

        public Type[] BeverageTypes { get; private set; }

        public Producer(Type[] BeverageTypes)
        {
            this.BeverageTypes = BeverageTypes;
            this.Buffer = new Buffer();
        }

        private void ProduceBeverages(object? state)
        {
            Random rng = new Random();
            
            while(Running)
            {
                int random = rng.Next(3);


                Beverage beverage = (Beverage) BeverageTypes[rng.Next(BeverageTypes.Length)].GetConstructors()[0].Invoke(new object[] { IdGenerator.GetNextId() });
                Buffer.AddBeverage(beverage);

                if(this.OnProduced != null)
                    this.OnProduced(beverage);

                Thread.Sleep(600);
            }
        }

        public void Start()
        {
            Running = true;
            ThreadPool.QueueUserWorkItem(ProduceBeverages);
        }

        public void Stop()
        {
            Running = false;
        }

    }
}
