using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using UserInfo;

namespace ChattingClient_1
{
    //패킷 정의
    public enum PACKETTYPE
    {
        eWELCOME = 1000,
        //사용자 정보
        eUSERINFO,
        eCHAT
    }
    public struct WELCOME
    {
        public int userID;
        public string message;
    }
    //패킷 구조체
    public struct USERINFO
    {
        public int userID;  //서버에서 할당한 ID
    } 
    class Program
    {
        static string strIp = "172.30.1.42";
        static int port = 8082;
        static User user;
        static List<User> userList;
        static void Main(string[] args)
        {
            //비동기 방식
            //1.소켓생성
            //2.Connect
            //3. 안녕하세요 라는 메시지를 서버로부터 수신 (할당받은 ID를 추가해서 전송)
            //4. 접속해있는 사용자 정보를 서버로부터 수신
            //5. 자신이 작성한 메세지를 서버로 송신
            //6. 서버로부터 자신이 보낸 메시지를 수신하여 콘솔뷰에 출력
            //서버로부터 결과를 받아서 콘솔뷰에 출력
            //7. 다른 사용자가 송신한 메시지를 콘솔뷰에 출력
            userList = new List<User>();
            user = new User(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
            
            user.userSock.Connect(ip);
            user.userSock.Receive(user.receiveBuffer);
            PacketParser();
            user.ClearReceiveBuffer();
            //비동기 방식으로 수신
            user.Receive();
            //user.userSock.Receive(user.receiveBuffer);
            //PacketParser();

            //string receiveMessage = Encoding.Default.GetString(user.receiveBuffer);
            //Console.WriteLine(receiveMessage);

            string userMessage = string.Empty;
            while (!userMessage.Contains("!"))
            {

                userMessage = Console.ReadLine();
                byte [] tmp = Encoding.Default.GetBytes(userMessage);
                byte[] _PACKETTYPE = BitConverter.GetBytes((ushort)PACKETTYPE.eCHAT);
                Array.Copy(_PACKETTYPE, 0, user.sendBuffer, 0, _PACKETTYPE.Length);
                Array.Copy(tmp, 0,user.sendBuffer, 2, tmp.Length);
                Array.Copy(tmp, user.sendBuffer, tmp.Length);
                //비동기 방식
                user.Send();
                /*Console.WriteLine(userMessage);
                sendBuffer = Encoding.Default.GetBytes(userMessage);
                clientSock.Send(sendBuffer);
                Console.WriteLine("보낸메세지 = " + userMessage);
                Array.Clear(sendBuffer, 0, sendBuffer.Length);
                clientSock.Receive(receiveBuffer);
                receiveMessage = Encoding.Default.GetString(receiveBuffer);
                Console.WriteLine("받은 메세지 = " + receiveMessage);*/
            }
            user.Close();
            //접속해있는 사용자 정보를 서버로부터 수신
            //이를 위해 패킷에 대한 약속? 정의가 필요함
        }

        public static void ReceiveCallBack(IAsyncResult ar)
        {
            PacketParser();
            user.Receive();
        }

        public static void SendCallBack(IAsyncResult ar)
        {
            user.ClearSendBuffer();
            user.Receive();
        }

        public static void PacketParser()
        {
            byte[] _PACKETTYPE = new byte[2];
            Array.Copy(user.receiveBuffer, 0, _PACKETTYPE, 0, _PACKETTYPE.Length);
            short packetType = BitConverter.ToInt16(_PACKETTYPE, 0);
            switch (packetType)
            {
                case (int)PACKETTYPE.eWELCOME:
                    {
                        byte[] _uid = new byte[4];      //id는 정수형이므로 4바이트
                        byte[] _message = new byte[122];//대화내용
                        Array.Copy(user.receiveBuffer, 2, _uid, 0, _uid.Length);
                        Array.Copy(user.receiveBuffer, 6, _message, 0, _message.Length);
                        int uid = BitConverter.ToInt32(_uid, 0);                //서버에서 할당한 아이디
                        string message = Encoding.Default.GetString(_message);  //환영메세지
                        Console.WriteLine(message);
                    }
                    break;

                case (int)PACKETTYPE.eUSERINFO:
                    {
                        byte[] _uid = new byte[4];      //id는 정수형이므로 4바이트
                        Array.Copy(user.receiveBuffer, 2, _uid, 0, _uid.Length);
                        int uid = BitConverter.ToInt32(_uid, 0);                //서버에서 할당한 아이디
                        User other = new User(uid);
                        userList.Add(other);
                        Console.WriteLine(uid+"님의 정보를 수신했습니다.");
                    }
                    break;
                case (int)PACKETTYPE.eCHAT:
                    {
                        Console.WriteLine("대화 내용을 수신했습니다.");
                    }
                    break;
            }
        }
    }
}
