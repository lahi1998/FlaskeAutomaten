using System.Threading;
using System;
using System.Runtime.ConstrainedExecution;
using System.Linq.Expressions;

static class Program
{


    static int countBeer, countSoda, count;
    static object[] Splitterarr = new object[10];
    static object[] BeerPort = new object[10];
    static object[] SodaPort = new object[10];
    static bool loadingtruck = false;

    static void Main()
    {

        Thread t = new Thread(Producer);
        t.Name = "Producer";
        t.Priority = ThreadPriority.Normal;
        t.Start();

        Thread t2 = new Thread(Splitter);
        t2.Name = "Splitter";
        t2.Priority = ThreadPriority.Normal;
        t2.Start();

        Thread t3 = new Thread(ConsumerBeer);
        t3.Name = "ConsumerBeer";
        t3.Priority = ThreadPriority.Normal;
        t3.Start();

        Thread t4 = new Thread(ConsumerSoda);
        t4.Name = "ConsumerSoda";
        t4.Priority = ThreadPriority.Normal;
        t4.Start();

        t.Join();
        t2.Join();
        t3.Join();
        t4.Join();




    }


    internal class Bottle
    {
        public string BottleName = BottleType();
        public string PantType = "a";
    }

    static string BottleType()
    {
        int rnd = Random.Shared.Next(0, 5);
        string type;

        if (rnd == 1)
        {
            type = "Beer";
            return type;
        }
        if (rnd == 2)
        {
            type = "Soda";
            return type;
        }
        if (rnd == 3)
        {
            type = "Soda";
            return type;
        }
        if (rnd == 4)
        {
            type = "Beer";
            return type;
        }
        else
        {
            type = "Beer";
            return type;

        }

    }



    static void Producer()
    {

        while (true)
        {
            Monitor.Enter(Splitterarr);
            try
            {
                CounterSplitterArray();
                if (count == 0 && loadingtruck == false)
                {
                    Console.WriteLine("Producer waits.");
                    Monitor.Wait(Splitterarr);
                }
                if (count == 10)
                {
                    for (int i = 0; i < Splitterarr.Length; i++)
                    {
                        if (Splitterarr[i] == null)
                        {
                            Bottle obj = new Bottle();

                            Splitterarr[i] = obj;
                            Console.WriteLine("Insert " + ((Bottle)Splitterarr[i]).BottleName);
                            Thread.Sleep(100);
                        }
                    }

                    Console.WriteLine("Producer waits.");
                    Monitor.PulseAll(Splitterarr);
                }
            }
            finally
            {
                Monitor.Exit(Splitterarr);
            }
        }
    }

    static void CounterSplitterArray()
    {
        int counter = 0;

        for (int i = 0; i < Splitterarr.Length; i++)
        {
            if (Splitterarr[i] == null)
            {
                counter++;
            }
        }

        count = counter;

    }


    static void Splitter()
    {
        while (true)
        {
            Monitor.Enter(Splitterarr);
            Monitor.Enter(BeerPort);
            Monitor.Enter(SodaPort);
            try
            {
                CounterSplitterArray();
                if (count == 10)
                {
                    Monitor.Wait(Splitterarr);
                }
                for (int i = 0; i < 10; i++)
                {
                    CounterPortArrays();
                    if (Splitterarr[i] != null && ((Bottle)Splitterarr[i]).BottleName == "Beer")
                    {
                        if (countBeer == 10)
                        {
                            loadingtruck = true;
                            Console.WriteLine("Splitter waits for Beer truck.");
                            Monitor.Wait(BeerPort);
                        }
                        for (int i1 = 0; i1 < BeerPort.Length; i1++)
                        {
                            if (BeerPort[i1] == null)
                            {
                                BeerPort[i1] = Splitterarr[i];
                                Console.WriteLine("Moved " + ((Bottle)BeerPort[i1]).BottleName);
                                Splitterarr[i] = null;
                                Thread.Sleep(100);
                                break;
                            }
                        }

                    }
                    if (Splitterarr[i] != null && ((Bottle)Splitterarr[i]).BottleName == "Soda")
                    {
                        if (countSoda == 10)
                        {
                            loadingtruck = true;
                            Console.WriteLine("Splitter waits for Soda truck.");
                            Monitor.Wait(SodaPort);
                        }
                        for (int i1 = 0; i1 < SodaPort.Length; i1++)
                        {
                            if (SodaPort[i1] == null)
                            {
                                SodaPort[i1] = Splitterarr[i];
                                Console.WriteLine("Moved " + ((Bottle)SodaPort[i1]).BottleName);
                                Splitterarr[i] = null;
                                Thread.Sleep(100);
                                break;
                            }
                        }

                    }
                }

                Console.WriteLine("Splitter waits.");
                Monitor.PulseAll(Splitterarr);
                Monitor.PulseAll(BeerPort);
                Monitor.PulseAll(SodaPort);

            }
            finally
            {
                Monitor.Exit(Splitterarr);
                Monitor.Exit(BeerPort);
                Monitor.Exit(SodaPort);
            }
        }
    }

    static void CounterPortArrays()
    {
        countBeer = 0;
        countSoda = 0;

        foreach (var bottle in BeerPort)
        {
            if (bottle != null)
            {
                countBeer++;
            }
        }

        foreach (var bottle in SodaPort)
        {
            if (bottle != null)
            {
                countSoda++;
            }
        }
    }

    static void ConsumerBeer()
    {
        bool run = true;
        while (run == true)
        {
            Monitor.Enter(BeerPort);
            try
            {
                if (loadingtruck == true)
                {
                    for (int i = 0; i < BeerPort.Length; i++)
                    {
                        if (BeerPort[i] != null)
                        {
                            Console.WriteLine("Loading {0} onto truck.", ((Bottle)BeerPort[i]).BottleName);
                            BeerPort[i] = null;
                            Thread.Sleep(100);
                        }
                    }
                    loadingtruck = false;
                    Console.WriteLine("Beer bottle truck drives away will return later.");
                    Monitor.PulseAll(BeerPort);
                }
            }
            finally
            {
                Monitor.Exit(BeerPort);
            }
        }
    }

    static void ConsumerSoda()
    {
        bool run = true;
        while (run == true)
        {
            Monitor.Enter(SodaPort);
            try
            {
                if (loadingtruck == true)
                {
                    for (int i = 0; i < SodaPort.Length; i++)
                    {
                        if (SodaPort[i] != null)
                        {
                            Console.WriteLine("Loading {0} onto truck.", ((Bottle)SodaPort[i]).BottleName);
                            SodaPort[i] = null;
                            Thread.Sleep(100);
                        }
                    }
                    loadingtruck = false;
                    Console.WriteLine("Soda bottle truck drives away will return later.");
                    Monitor.PulseAll(SodaPort);
                }
            }
            finally
            {
                Monitor.Exit(SodaPort);
            }
        }
    }

}
