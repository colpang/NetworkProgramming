using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UserInfo;

namespace ChattingServer_1
{   //패킷 정의
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
        static Socket listenSock;
        static string strIp = "172.30.1.42";
        static int port = 8082;
        static Thread t1;
        static bool isInterrupt;
        static bool isInterruptMain;
        static List<User> userList;
        static void Main(string[] args)
        {
            userList = new List<User>();
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
                try
                {
                    listenSock.Listen(100);     //소켓을 수신상태로 설정, 100개까지의 수신 대기
                                                //Socket userSock = listenSock.Accept();  //새로 만든 연결에 대한 새 소켓 할당
                    listenSock.BeginAccept(AcceptCallBack, null);
                    Thread.Sleep(10);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket userSock = listenSock.EndAccept(ar);
            User user = new User(userSock);
            Console.WriteLine("접속한 사용자 = " + userSock.RemoteEndPoint + " ID = " + userSock.Handle);
            WelcomePacket(user);
            SendUserInfos(user);
            userList.Add(user);
            user.Receive();
            //user.ClearBuffer();
            //userSock.BeginReceive(receiveBuffer,0,receiveBuffer.Length,SocketFlags.None,ReceiveCallBack, userSock);
        }
        static void WelcomePacket(User _user)
        {
            _user.ClearBuffer();
            //eWELCOME
            byte[] _PACKETTYPE = BitConverter.GetBytes((ushort)PACKETTYPE.eWELCOME);
            byte[] _uid = BitConverter.GetBytes((int)_user.userSock.Handle);
            byte[] _message = Encoding.Default.GetBytes("안녕하세요");
            //버퍼에 복사 
            Array.Copy(_PACKETTYPE, 0, _user.sendBuffer, 0, _PACKETTYPE.Length);
            Array.Copy(_uid, 0, _user.sendBuffer, 2, _uid.Length);
            Array.Copy(_message, 0, _user.sendBuffer, 6, _message.Length);
            _user.SendSyncronous();
        }
        static void SendUserInfos(User _user)
        {
            _user.ClearBuffer();
            //1.접속한 유저에게 다른 사람의 정보를 전송
            foreach (User one in userList)
            {
                //eWELCOME
                byte[] _PACKETTYPE = BitConverter.GetBytes((ushort)PACKETTYPE.eUSERINFO);
                byte[] _uid = BitConverter.GetBytes((int)one.userSock.Handle);
                //버퍼에 복사 
                Array.Copy(_PACKETTYPE, 0, _user.sendBuffer, 0, _PACKETTYPE.Length);
                Array.Copy(_uid, 0, _user.sendBuffer, 2, _uid.Length);
                _user.SendSyncronous();
            }
            //2. 다른 유저에게 현재 접속한 사람을 전송
            foreach (User one in userList)
            {
                byte[] _PACKETTYPE = BitConverter.GetBytes((ushort)PACKETTYPE.eUSERINFO);
                byte[] _uid = BitConverter.GetBytes((int)_user.userSock.Handle);
                Array.Copy(_PACKETTYPE, 0, one.sendBuffer, 0, _PACKETTYPE.Length);
                Array.Copy(_uid, 0, one.sendBuffer, 2, _uid.Length);
                one.SendSyncronous();
            }
        }
        public static void RemoveUser(User _user)
        {
            userList.Remove(_user);
        }
        public static void PacketParsr(User _user)
        {
            //패킷을 분해
            byte[] _PACKETTYPE = new byte[2];
            Array.Copy(_user.receiveBuffer, 0, _PACKETTYPE, 0, _PACKETTYPE.Length);
            short packetType = BitConverter.ToInt16(_PACKETTYPE, 0);
            switch (packetType)
            {
                case 0:
                //    try
                //    {
                //        User finduser = null;
                //        for(int i=0; i<userList.Count; i++)
                //        {
                //            if (userList[i].sockHandle == 0 || userList[i].userSock == null || userList[i].sockHandle == _user.sockHandle || userList[i].userSock.Connected == false)
                //            {
                //                finduser = userList[i];
                //                break;
                //            }
                //        }
                //        userList.Remove(finduser);
                       _user.Close();
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message);
                //        /* foreach(User one in userList)
                //         {
                //             if(one.sockHandle == 0 || one.userSock == null ||one.sockHandle == _user.sockHandle ||one.userSock.Connected == false)
                //             {
                 //                userList.Remove(one);
                 //                break;
                //             }
                 //        }*/
                 //       User[] array = userList.ToArray();
                 //       for(int i = 0; i < array.Length; i++)
                 //       {
                 //           if(array[i].sockHandle == _user.sockHandle)
                //            {
                 //               userList.Remove(array[i]);
                //            }
                 //       }
                  //      _user.Close();
                  //  }
                    break;
                case (int)PACKETTYPE.eCHAT:
                    //user의 수신 버퍼에 있는 내용을 모든 유저의 송신버퍼로 복사한 후 전송
                    for (int i=0;i<userList.Count; i++)
                    {
                        Array.Copy(_user.receiveBuffer, userList[i].sendBuffer, _user.receiveBuffer.Length);
                        userList[i].Send();
                    }
                    Array.Clear(_user.receiveBuffer, 0, _user.receiveBuffer.Length);
                    break;
            }
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
                //user.CopySendBufFromReceiveBuf();
                //user.Send();
                //받은 내용을 모든 사용자에게 전송
                // Console.WriteLine("대화내용수신함");
                PacketParsr(user);

            }
            catch(Exception e)
            {
                user.Close();
            }
            /*catch (SocketException e)
            {
                User findUser = userList.Find(o => o.Equals(user));
                if (findUser != null)
                {
                    
                }
                try
                {
                    user.Close();
                }
                catch (ObjectDisposedException e1)
                {
                    Console.WriteLine(e1.Message);
                }
            }
            catch (ObjectDisposedException e)
            {

            }*/

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
            catch (SocketException e)
            {
                user.Close();
            }

        }
    }
}
