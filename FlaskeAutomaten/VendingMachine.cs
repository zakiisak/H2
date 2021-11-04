using FlaskeAutomaten.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomaten
{
    public class VendingMachine
    {
        public Producer Producer { get; private set; }
        public Splitter Splitter { get; private set; }
        public Consumer[] Consumers { get; private set; }

        public bool IsOn { get; private set; }

        public VendingMachine()
        {
            Type[] beverageTypes = new Type[] { typeof(Beer), typeof(Fanta), typeof(Cocio), typeof(Cola), typeof(IceTea), typeof(FaxeKondi) };
            this.Producer = new Producer(beverageTypes);


            KeyValuePair<Type, Buffer>[] splitterBuffers = new KeyValuePair<Type, Buffer>[beverageTypes.Length];
            this.Consumers = new Consumer[beverageTypes.Length];

            for(int i = 0; i < beverageTypes.Length; i++)
            {
                Buffer buffer = new Buffer();
                splitterBuffers[i] = new KeyValuePair<Type, Buffer>(beverageTypes[i], buffer);
                this.Consumers[i] = new Consumer(buffer, beverageTypes[i]);
            }
            this.Splitter = new Splitter(Producer.Buffer, splitterBuffers);
        }

        public void TurnOn()
        {
            if (IsOn == false)
            {
                IsOn = true;
                Producer.Start();
                Splitter.Start();
                foreach (Consumer consumer in Consumers)
                    consumer.Start();
            }
        }

        public void TurnOff()
        {
            IsOn = false;
            Producer.Stop();
            Splitter.Stop();
            foreach (Consumer consumer in Consumers)
                consumer.Stop();
        }
    }
}
