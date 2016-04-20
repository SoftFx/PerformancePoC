using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumer
{
    class ProducerConsumer
    {
        object stateLock = new object();
        LinkedList<Program> queue = new LinkedList<Program>();

        long consumerCounter = 0;
        long producerCounter = 0;

        public void Init()
        {
            Thread workingThread = new Thread(Consumer);
            workingThread.Start();
            OutputStatistics(this);
            Thread producer = new Thread(RunProducer);
            producer.Start();
        }

        public void RunProducer()
        {
           // Console.WriteLine("Producer thread id: " + AppDomain.GetCurrentThreadId().ToString());
            long i = 0;
            while (true)
            {
                lock (stateLock)
                {
                    if (queue.Count == 0)
                        queue.AddLast(new Program());
                    else
                        queue.First.Value = new Program();

                    producerCounter++;
                    Monitor.Pulse(stateLock);
                }
//                if( i++ % 2 ==0)
                    Thread.Sleep(1);

            }
        }
        void Consumer()
        {
            //Console.WriteLine("Consumer thread id: " + AppDomain.GetCurrentThreadId().ToString());
            while (true)
            {
                lock (stateLock)
                {
                    Monitor.Wait(stateLock);
                    var debris = queue.First.Value;
                    queue.RemoveFirst();

                    consumerCounter += 1;
                }
            }
        }
        public void OutputStatistics(object state)
        {
            long produced = 0;
            long consumed = 0;

            lock (stateLock)
            {
                consumed = consumerCounter;
                produced = producerCounter;
                consumerCounter = producerCounter = 0;
            }
            Console.WriteLine("Produces {1}. Consumed {0}.", consumed, produced);
            Task.Delay(30000).ContinueWith(OutputStatistics);
        }
    }
}
