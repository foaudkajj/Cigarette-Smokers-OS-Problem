using System;
using System.Threading;

namespace ConsoleApplication1
{
    class CSwithMutex
    {
        // Mut : this mutex will be used by agents to see if any agent is putting ingredients or if any 
        // smoker is making its cigarette ( to check the freeness of the table )
        private static Mutex Mut = new Mutex(false);

        //We have here varibales are using to make signaling between smokers and agents
        private static AutoResetEvent signalFromA = new AutoResetEvent(false);
        private static AutoResetEvent signalFromB = new AutoResetEvent(false);
        private static AutoResetEvent signalFromC = new AutoResetEvent(false);
        
        //Ingredients Value
        private static int tobacco = 0, match = 0, paper = 0;

        //This method will be invoked by the main menu
        public void Beginning()
        {
            // Threads for the agents and the smokers
            Thread StartAgents = new Thread(startAgents);
            Thread smokerMatchFun = new Thread(smokerMatch);
            Thread smokerTbaccoFun = new Thread(smokerTobbaco);
            Thread smokerPaperFun = new Thread(smokerPaper);

            //Starting threads .
            smokerMatchFun.Start();
            smokerTbaccoFun.Start();
            smokerPaperFun.Start();
            StartAgents.Start();

        }
        static void startAgents()
        {
            while (true)
            {
                //waiting the mutex to be free
                Mut.WaitOne();
                int random = new Random().Next(1,4);
                if (random == 1)
                {
                    Console.WriteLine("Now is Agent A with paper and tobacco .");
                    Console.WriteLine($"Tobacco is Active and its value is : {++tobacco} ");
                    Console.WriteLine($"Paper is Active and its value is : {++paper}");
                    Thread.Sleep(new Random().Next(100, 2000));
                    Console.WriteLine("Wakeup signal sent to Match Smokker");
                    //Signal to the proper smoker
                    signalFromA.Set();

                }
                else if (random == 2)
                {
                    Console.WriteLine("Now is agent B with paper and matches .");
                    Console.WriteLine($"Match is Active and its value is : {++match}");;
                    Console.WriteLine($"Paper is Active and its value is : {++paper}");
                    Thread.Sleep(new Random().Next(100, 2000));
                    Console.WriteLine("Wakeup signal sent to Tobacco Smokker");
                    signalFromB.Set();
                }
                else
                {
                    Console.WriteLine("Now is agent C with tobacco and matches .");
                    Console.WriteLine($"Match is Active and its value is : {++match}");
                    Console.WriteLine($"Tobacco is Active and its value is : {++tobacco}");
                    Thread.Sleep(new Random().Next(100, 2000));
                    Console.WriteLine("Wakeup signal sent to Paper Smokker");
                    signalFromC.Set();
                }
                //Freeing the mutex
                Mut.ReleaseMutex();
                
            }

            
        }

        static void smokerMatch()
        {
            while (true)
            {
                // Here the smoker can not start before receiving the signal from the Agent
                signalFromA.WaitOne();
                //After receiving the signal it will wait until the mutex ( waiting the table )
                Mut.WaitOne();

                Console.WriteLine($"Smoker Match is making Cigarette by Tobacco and Paper . \n Tobacco value now is : {--tobacco} , Paper value now is : {--paper}");
                Thread.Sleep(new Random().Next(100, 2000));

                // Releasing the mutex (table )
                Mut.ReleaseMutex();
                //Resetting the signal to wait another signal from the agent .
                signalFromA.Reset();
                Console.WriteLine("Smoker Match is smoking  ....");
                Thread.Sleep(new Random().Next(100, 2000));
            }
        }

        static void smokerTobbaco()
        {
            while (true)
            {
                signalFromB.WaitOne();
                Mut.WaitOne();
                Console.WriteLine($"Smoker Tobacco is making Cigarette by Match and Paper .\n Match value now is : {--match} , Paper value now is : {--paper}");
                Thread.Sleep(new Random().Next(100, 2000));
                signalFromB.Reset();
                Mut.ReleaseMutex();
                Console.WriteLine("Smoker Tobacco is smoking  ....");
                Thread.Sleep(new Random().Next(100, 2000));
            }

        }


        static void smokerPaper()
        {
            while (true)
            {
                signalFromC.WaitOne();
                Mut.WaitOne();
                Console.WriteLine($"Smoker Paper is making Cigarette by Match and Tobacco .\n Match value now is : {--match} , Tobacco value now is : {--tobacco}");
                Thread.Sleep(new Random().Next(100, 2000));
                signalFromC.Reset();
                Mut.ReleaseMutex();
                Console.WriteLine("Smoker Paper is smoking  ....");
                Thread.Sleep(new Random().Next(100, 2000));
            }

        }

    }
}
