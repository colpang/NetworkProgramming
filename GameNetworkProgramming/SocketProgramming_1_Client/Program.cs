using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketProgramming_1_Client
{
    class Program
    {
        static Socket clientSock;
        static string strIp = "172.30.1.42";
        static int port = 8082;
        static void Main(string[] args)
        {
            try
            {
                clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
                clientSock.Connect(ip);     //원격 호스트에 대한 연결을 설정
                                            //서버가 보낸 메시지를 변환하여 수신
                byte[] receiveBuffer = new byte[128];
                clientSock.Receive(receiveBuffer);
                string receiveMessage = Encoding.Default.GetString(receiveBuffer);
                Console.WriteLine(receiveMessage);
                Array.Clear(receiveBuffer,0,receiveBuffer.Length);
                string userMessage = string.Empty;
                byte[] sendBuffer = new byte[128];
                while(!userMessage.Contains("!"))
                {
                    userMessage = Console.ReadLine();
                    Console.WriteLine(userMessage);
                    sendBuffer = Encoding.Default.GetBytes(userMessage);
                    clientSock.Send(sendBuffer);
                    Console.WriteLine("보낸메세지 = "+userMessage);
                    Array.Clear(sendBuffer,0,sendBuffer.Length);
                    clientSock.Receive(receiveBuffer);
                    receiveMessage = Encoding.Default.GetString(receiveBuffer);
                    Console.WriteLine("받은 메세지 = "+receiveMessage);
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
