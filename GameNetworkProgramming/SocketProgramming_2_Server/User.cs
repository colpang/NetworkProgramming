using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using SocketProgramming_2_Server;
using System.Threading;
public class User
{
    //소켓
    //송신버퍼
    //수신버퍼
    public Socket userSock;
    public byte[] sendBuffer;
    public byte[] receiveBuffer;
    private const short MAXBUFSIZE = 128;
    List<User> userList;
    Thread t;
    public User(Socket _sock, List<User> _userList)
    {
        userSock = _sock;
        sendBuffer = new byte[MAXBUFSIZE];
        receiveBuffer = new byte[MAXBUFSIZE];
        userList = _userList;
        ThreadStart threadstart = new ThreadStart(NewClient);
        t = new Thread(threadstart);
        t.Start();
    }
    public void NewClient()
    {
        while (true)
        {
            try
            {
                Receive();
                ClearSendBuffer();
                CopySendBufFromReceiveBuf();
                Send();
                ClearReceiveBuffer();
                Receive();
                Thread.Sleep(10);
            }
            catch (SocketException e)
            {
                Close();
                userList.Remove(this);
                Console.WriteLine(e.Message);
                t.Join();
                t.Interrupt();
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {

            }
        }
    }
    public void Close()
    {
        //객체가 메모리에서 사라질 때 호출
        Console.WriteLine(userSock.RemoteEndPoint + "님이 접속종료했습니다.");
        userSock.Shutdown(SocketShutdown.Both);
        userSock.Close();
    }
    public void Receive()
    {
        userSock.Receive(receiveBuffer);
        Program.messageQueue.Enqueue(this);
        
    }
    public void Send()
    {
        userSock.Send(sendBuffer);
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
        Array.Copy(receiveBuffer,sendBuffer, receiveBuffer.Length);
    }
}