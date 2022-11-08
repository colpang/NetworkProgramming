using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadExample
{
    class Program
    {
        static Thread t1;
        static void Main(string[] args)
        {
            ThreadStart threadstart = new ThreadStart(NewClient);
            t1 = new Thread(threadstart);
            t1.Start();
            int count = 0;
            while (count < 300)
            {
                count++;
                Console.WriteLine("Main함수:"+count);
            }
            t1.Join();
            Console.WriteLine("Join");
            t1.Interrupt();
            Console.WriteLine("Interrupt");

            Socket sock = new Socket(SocketType.Stream, ProtocolType.Udp);
        }
        static void NewClient()
        {
            int threadCount = 0;
            while (threadCount<300)
            {
                threadCount++;
                Console.WriteLine("Thread 함수 = "+threadCount);
                Thread.Sleep(10);
            }
        }
    }
}
