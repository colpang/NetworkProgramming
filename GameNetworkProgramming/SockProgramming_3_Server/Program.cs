using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SockProgramming_3_Server
{
    class Program
    {
        static Socket listenSock;
        static string strIp = "172.30.1.42";
        static int port = 8082;
        static Thread t1;
        static bool isInterrupt;
        static bool isInterruptMain;
        static byte[] sendBuffer;
        static byte[] receiveBuffer;
        static void Main(string[] args)
        {
            sendBuffer = new byte[128];
            receiveBuffer = new byte[128];
            listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
            listenSock.Bind(ip);        //IP와 Port 할당
            Console.WriteLine("bind");
            ThreadStart threadstart = new ThreadStart(NewClient);
            t1 = new Thread(threadstart);
            t1.Start();
            Console.WriteLine("쓰레드시작");
            t1.Join();
            t1.Interrupt();
        }
        static void NewClient()
        {
            while (!isInterrupt)
            {
                listenSock.Listen(100);     //소켓을 수신상태로 설정, 100개까지의 수신 대기
                Console.WriteLine("listen");
                //Socket userSock = listenSock.Accept();  //새로 만든 연결에 대한 새 소켓 할당
                listenSock.BeginAccept(AcceptCallBack, null);
                Thread.Sleep(10);
            }
        }

        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket userSock = listenSock.EndAccept(ar);
            Console.WriteLine("접속한 사용자 = "+userSock.RemoteEndPoint + " ID = "+userSock.Handle);
            string message = "안녕하세요.";
            byte[] tmp = Encoding.Default.GetBytes(message);
            userSock.Send(tmp);
            User user = new User(userSock);
            user.Receive();
            //userSock.BeginReceive(receiveBuffer,0,receiveBuffer.Length,SocketFlags.None,ReceiveCallBack, userSock);
        }

        public static void ReceiveCallBack(IAsyncResult ar)
        {
            User user = (User)ar.AsyncState;
            //ar에 userSock이 넘어옴
            /*Socket userSock = (Socket)ar.AsyncState;
            Array.Copy(receiveBuffer, sendBuffer, receiveBuffer.Length);
            Array.Clear(receiveBuffer,0,receiveBuffer.Length);
            userSock.BeginSend(sendBuffer,0,sendBuffer.Length,SocketFlags.None,SendCallBack,userSock);
            */
            try
            {
                user.CopySendBufFromReceiveBuf();
                user.Send();
            }
            catch (SocketException e)
            {
                user.Close();
            }
        }
        public static void SendCallBack(IAsyncResult ar)
        {
            //매개변수가 다시 전달
            /*Socket userSock = (Socket)ar.AsyncState;
            int sendLength = userSock.EndSend(ar);
            Array.Clear(sendBuffer, 0, sendBuffer.Length);
            userSock.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallBack, userSock);*/
            User user = (User)ar.AsyncState;
            try
            {
                user.ClearSendBuffer();
                user.Receive();
            }
            catch(SocketException e)
            {
                user.Close();
            }

        }
    }
}
