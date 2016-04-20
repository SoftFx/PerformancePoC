using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumer
{
    class Program
    {


        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                ProducerConsumer p = new ProducerConsumer();
                p.Init();
            }

            Console.ReadLine();
        }


    }
}
