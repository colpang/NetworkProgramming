using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChattingServer_1;

namespace UserInfo
{
    public class User
    {
        //소켓
        //송신버퍼
        //수신버퍼
        public Socket userSock;
        public int sockHandle;
        public byte[] sendBuffer;
        public byte[] receiveBuffer;
        private const short MAXBUFSIZE = 128;
        List<User> userList;
        Thread t;
        public User(Socket _sock)
        {
            userSock = _sock;
            sockHandle = (int)_sock.Handle;
            sendBuffer = new byte[MAXBUFSIZE];
            receiveBuffer = new byte[MAXBUFSIZE];
        }

        public void Close()
        {
            //객체가 메모리에서 사라질 때 호출
            Program.RemoveUser(this);
            Console.WriteLine(userSock.RemoteEndPoint + "님이 접속종료했습니다.");
            userSock.Shutdown(SocketShutdown.Both);
            userSock.Close();
        }
        public void SendSyncronous()
        {
            userSock.Send(sendBuffer);
        }
        public void Receive()
        {
            //userSock.Receive(receiveBuffer);
            userSock.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, Program.ReceiveCallBack, this);

        }
        public void Send()
        {
            //userSock.Send(sendBuffer);
            userSock.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, Program.SendCallBack, this);
        }
        public void ClearSendBuffer()
        {
            Array.Clear(sendBuffer, 0, sendBuffer.Length);
        }
        public void ClearReceiveBuffer()
        {
            Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
        }
        public void CopySendBufFromReceiveBuf()
        {
            Array.Copy(receiveBuffer, sendBuffer, receiveBuffer.Length);
        }
        public void ClearBuffer()
        {
            ClearSendBuffer();
            ClearReceiveBuffer();
        }
    }
}
