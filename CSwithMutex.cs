using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApplication1
{
    class CSwithMutex
    {
        //Here i have 4 Mutexes 
        // agentMut : this mutex will be used by agents to see if any thread is putting ingredients or if any 
        // smoker is making its cigarette
        private static Mutex agentsMut = new Mutex(true);
        private static Mutex tobbacoMut = new Mutex(true);
        private static Mutex paperMut = new Mutex(true);
        private static Mutex matchMut = new Mutex(true);
        //We have here varibales are using to make signaling between smokers and agents
        private static AutoResetEvent signalFromA = new AutoResetEvent(false);
        private static AutoResetEvent signalFromB = new AutoResetEvent(false);
        private static AutoResetEvent signalFromC = new AutoResetEvent(false);
        private static AutoResetEvent signalToA = new AutoResetEvent(true);
        private static AutoResetEvent signalToB = new AutoResetEvent(true);
        private static AutoResetEvent signalToC = new AutoResetEvent(true);

        public CSwithMutex()
        {
            // While the program is loading i have to release these mutexes to give the ability to threads to 
            // acquire them for the first time
            agentsMut.ReleaseMutex();
            tobbacoMut.ReleaseMutex();
            paperMut.ReleaseMutex();
            matchMut.ReleaseMutex();

        }
        public void startAgents()
        {
            // For every Agent and Smoker i have one thread
            Thread A = new Thread(startAgentA);
            Thread B = new Thread(startAgentB);
            Thread C = new Thread(startAgentC);
            Thread smokerMatchFun = new Thread(smokerMatch);
            Thread smokerTbaccoFun = new Thread(smokerTobbaco);
            Thread smokerPaperFun = new Thread(smokerPaper);



            A.Start();
            B.Start();
            C.Start();
            smokerMatchFun.Start();
            smokerTbaccoFun.Start();
            smokerPaperFun.Start();



            #region TestingCode
            //Thread toabacoSmokerThread = new Thread(smokerTobbaco);
            //toabacoSmokerThread.Start();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(startAgentC), autoEvent);
            // autoEvent.WaitOne(); 
            #endregion
        }

        #region TestingCode
        //static void startAgentC(object stateInfo)
        //{
        //    Console.WriteLine("Now is C with tobbaco and matches .");
        //    agentsMut.WaitOne();
        //    Thread.Sleep(new Random().Next(100, 2000));
        //    agentsMut.ReleaseMutex();
        //    ((AutoResetEvent)stateInfo).Set();
        //} 
        #endregion
        static void startAgentA()
        {
            while (true)
            {
                try
                {
                    //Here we are waiting the signal from smoker .
                    //That signal tells this Agent that the smoking is finish and you can put another ingredients
                    signalToA.WaitOne();
                    // Agent A is waiting the Agent Mutex to be free
                    agentsMut.WaitOne();
                    Console.WriteLine("Now is Agent A with paper and tobacco .");
                    // Agent A is waiting Tobacco to be free
                    tobbacoMut.WaitOne();
                    Console.WriteLine("Tobacco is Active .");
                    // Agent is waiting Paper Mutex to be free
                    paperMut.WaitOne();
                    Console.WriteLine("Paper is Active .");
                    // Now proper ingredients are available on the table so we can signal 
                    // the smoker to start its making cigarette and smoking
                    signalFromA.Set();
                    // after making the cigarette we are now able to release our paper
                    // and tobacco and agent mutex .
                    // releasing paper and tobacco mutex giving the ability to another agent to use them .
                    // releasing agent mutex giving the ability to another agent to start .
                    paperMut.ReleaseMutex();
                    tobbacoMut.ReleaseMutex();
                    agentsMut.ReleaseMutex();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //smokerMatch(stateInfo);
        }

        #region TestingCode
        //static void MedA()
        //{
        //    matchMut.WaitOne();

        //    agentsMut.WaitOne();
        //    tobbacoMut.WaitOne();
        //    paperMut.WaitOne();
        //    agentsMut.ReleaseMutex();

        //    Console.WriteLine("Making is completed by Tobacco and Paper .");
        //    smokerMatch();

        //} 
        #endregion

        static void smokerMatch()
        {
            while (true)
            {
                // Here the smoker can not start before receiving the signal from the Agent
                // that indicates that the ingredients are on the table
                signalFromA.WaitOne();
                // Now in this line the smoker is making its cigarette and smoking
                Console.WriteLine("Smoker Match is making Cigarette by Tobacco and Paper .");
                Thread.Sleep(new Random().Next(100, 2000));
                Console.WriteLine("Smoker Match is smoking  ....");
                Thread.Sleep(new Random().Next(100, 4000));
                // After consuming ingredients and smoking the smoker will reset the signal that comes from the agent
                // and put it unsignaled to wait another singal when ingredients are on the table for new time
                signalFromA.Reset();
                // Also after finishing smoking , The smoker has to tell the Agent that i have finished my ingredients and smoked my 
                //cigarette so you can put another ingredients when you ca .
                signalToA.Set();
            }
        }

        #region TestingCode
        //static void smokingMatch()//object stateInfo)
        //{

        //    //((AutoResetEvent)stateInfo).Set();
        //} 
        #endregion













        static void startAgentB()
        {
            while (true)
            {
                try
                {
                    signalToB.WaitOne();
                    agentsMut.WaitOne();
                    Console.WriteLine("Now is agent B with paper and matches .");
                    matchMut.WaitOne();
                    Console.WriteLine("Match is Active .");
                    paperMut.WaitOne();
                    Console.WriteLine("Paper is Active .");
                    signalFromB.Set();
                    matchMut.ReleaseMutex();
                    paperMut.ReleaseMutex();
                    agentsMut.ReleaseMutex();
                    Thread smokerTobaccoFun = new Thread(smokerTobbaco);
                    smokerTobaccoFun.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }


        }


        #region TestingCode
        //static void MedB()
        //{
        //    Console.WriteLine("Smoker is making Cigarette by Match and Paper .");
        //    Thread.Sleep(new Random().Next(100, 2000));
        //    Console.WriteLine("Making Cigarette is completed by Cigarette and Match .");
        //    agentsMut.ReleaseMutex();
        //    smokerTobbaco();

        //} 
        #endregion
        static void smokerTobbaco()//object stateInfo)
        {
            signalFromB.WaitOne();
            Console.WriteLine("Smoker Tobacco is making Cigarette by Match and Paper .");
            Thread.Sleep(new Random().Next(100, 2000));
            Console.WriteLine("Smoker Tobacco is smoking  ....");
            Thread.Sleep(new Random().Next(100, 4000));
            signalFromB.Reset();
            signalToB.Set();

        }

        #region TestingCode
        //static void smokingTobbaco()//object stateInfo)
        //{

        //    // ((AutoResetEvent)stateInfo).Set();
        //} 
        #endregion


        static void startAgentC()//object stateInfo)
        {
            while (true)
            {
                try
                {
                    signalToC.WaitOne();
                    agentsMut.WaitOne();
                    Console.WriteLine("Now is agent C with tobacco and matches .");
                    matchMut.WaitOne();
                    Console.WriteLine("Match is Active .");
                    tobbacoMut.WaitOne();
                    Console.WriteLine("Tobacco is Active .");
                    signalFromC.Set();
                    matchMut.ReleaseMutex();
                    tobbacoMut.ReleaseMutex();
                    agentsMut.ReleaseMutex();
                    Thread smokerTobaccoFun = new Thread(smokerPaper);
                    smokerTobaccoFun.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void smokerPaper()//object stateInfo)
        {
            signalFromC.WaitOne();
            Console.WriteLine("Smoker Paper is making Cigarette by Match and Tobacco .");
            Thread.Sleep(3000);
            Console.WriteLine("Smoker Paper is smoking  ....");
            Thread.Sleep(new Random().Next(100, 4000));
            signalFromC.Reset();
            signalToC.Set();

        }

    }
}
