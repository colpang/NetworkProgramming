using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SocketProgramming_2_Server
{
    class Program
    {
        static Socket listenSock;
        static string strIp = "172.30.1.42";
        static int port = 8082;
        static Thread t1;
        static bool isInterrupt;
        static bool isInterruptMain;
        static List<User> userList;
        static public Queue<User> messageQueue;
        static void Main(string[] args)
        {
            isInterrupt = false;
            isInterruptMain = false;
            userList = new List<User>();
            messageQueue = new Queue<User>();
            listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
            listenSock.Bind(ip);        //IP와 Port 할당
            Console.WriteLine("bind");          
            ThreadStart threadstart = new ThreadStart(NewClient);
            t1 = new Thread(threadstart);
            t1.Start();
            //임시
            //사용자마다 버퍼가 각각 존재해야한다.
            byte[] receiveBuffer = new byte[128];
            byte[] sendBuffer = new byte[128];
            while (!isInterruptMain) 
            {
                try
                {
                }
                catch(SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch(ObjectDisposedException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {

                }
            }
            t1.Join();
            t1.Interrupt();
        }

        static void NewClient()
        {
            while (!isInterrupt)
            {
                listenSock.Listen(100);     //소켓을 수신상태로 설정, 100개까지의 수신 대기
                Console.WriteLine("listen");
                Socket userSock = listenSock.Accept();  //새로 만든 연결에 대한 새 소켓 할당
                User user = new User(userSock, userList);
                userList.Add(user);
                Console.WriteLine("accept");
                string message = "안녕하세요.";
                byte[] tmp = Encoding.Default.GetBytes(message);
                userSock.Send(tmp);
                Thread.Sleep(10);
            }
        }
    }
}
