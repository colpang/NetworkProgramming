using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public enum ePACKETTYPE
{
    NONE,
    PEERINFO,
    CHARSELECT
}
public struct PEERINFO
{
    public int uid;
    public string name;
    public short charType;
}
delegate void Do();
public class P2PServer : MonoBehaviour
{
    Socket listenSock;
    string IP;
    int port;
    byte[] sBuffer;
    byte[] rBuffer;
    GameObject myChar;
    GameObject OtherChar;

    Do doCreate;
    Queue<byte[]> packetQueue;
    ePACKETTYPE ePacketType;
    private void Awake()
    {
        IP = "172.30.1.42";
        port = 8082;
        sBuffer = new byte[128];
        rBuffer = new byte[128];
        packetQueue = new Queue<byte[]>();
        doCreate = null;
    }


    // Start is called before the first frame update
    void Start()
    {
        listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP), port);
        listenSock.Bind(ip);
        listenSock.Listen(10);
        listenSock.BeginAccept(AddPeer, null);
    }


    public void CreateGameObject()
    {
        GameObject tmp = Resources.Load<GameObject>("Cube");
        if (tmp != null)
        {
            OtherChar = GameObject.Instantiate<GameObject>(tmp);
        }
        myChar = GameObject.Instantiate<GameObject>(tmp);
        doCreate -= CreateGameObject;
    }
    public void AddPeer(IAsyncResult ar)
    {
        Socket otherPeer = listenSock.EndAccept(ar);
        Debug.Log(otherPeer.RemoteEndPoint + "님이 접속했습니다.");
        doCreate = CreateGameObject;
        listenSock.BeginReceive(rBuffer , 0, rBuffer.Length,SocketFlags.None, ReceiveCallBack ,null);
    }
    public void ReceiveCallBack(IAsyncResult ar)
    {
        //queue에 enqueue
        byte[] tmp = new byte[128];
        Array.Copy(rBuffer, tmp, rBuffer.Length);
        Array.Clear(rBuffer,0,rBuffer.Length);
        packetQueue.Enqueue(tmp);
        listenSock.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, null);
    }
    // Update is called once per frame
    void Update()
    {
        if(doCreate != null)
        {
            doCreate();
        }
        if(packetQueue.Count > 0)
        {
            byte[] data = packetQueue.Dequeue();
            //패킷타입(2) + 패킷 내용(126)
            byte[] _Packet = new byte[2];
            Array.Copy(data,0,_Packet,0,2);
            short packetType =BitConverter.ToInt16(_Packet,0) ;
            switch ((int)packetType)
            {
                case (int)ePACKETTYPE.PEERINFO:
                    break;
                case (int)ePACKETTYPE.CHARSELECT:
                    {

                    }
                    break;
            }
        }
    }
}
