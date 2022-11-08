using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
 
namespace SocketProgramming_1_Server
{
    class Program
    {
        static Socket listenSock;
        static string strIp = "172.30.1.42";
        static int port = 8082;
        static void Main(string[] args)
        {            
            listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
            listenSock.Bind(ip);        //IP와 Port 할당
            Console.WriteLine("bind");
            listenSock.Listen(100);     //소켓을 수신상태로 설정, 100개까지의 수신 대기
            Console.WriteLine("listen");
            try
            {
                Socket userSock = listenSock.Accept();  //새로 만든 연결에 대한 새 소켓 할당
                //새로 접속한 유저에게 안녕하세요 메시지를 송신
                //메시지를 바이트로 변환하여 전송
                string message = "안녕하세요.";
                byte[] tmp =Encoding.Default.GetBytes(message);
                userSock.Send(tmp);
                Console.WriteLine("accept");
                Console.WriteLine("접속 유저 = "+userSock.RemoteEndPoint);
                while (true)
                {
                    byte[] receiveBuffer = new byte[128];
                    byte[] sendBuffer = new byte[128];
                    userSock.Receive(receiveBuffer);
                    Array.Clear(sendBuffer, 0, sendBuffer.Length);
                    Array.Copy(receiveBuffer, sendBuffer, receiveBuffer.Length);
                    userSock.Send(sendBuffer);
                    Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {

            }

            
        }
    }
}
