using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

delegate void Do();
public class P2PClient : MonoBehaviour
{
    Socket ClientSock;
    GameObject otherChar;
    GameObject myChar;
    string IP;
    int port;
    byte[] sBuffer;
    byte[] rBuffer;

    Do doCreate;
    private void Awake()
    {
        IP = "172.30.1.42";
        port = 8082;
        sBuffer = new byte[128];
        rBuffer = new byte[128];
    }
    // Start is called before the first frame update
    void Start()
    {
        ClientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP), port);
        ClientSock.Connect(ip);

        GameObject tmp = Resources.Load<GameObject>("Cube");
        if (tmp != null)
        {
            myChar = GameObject.Instantiate<GameObject>(tmp);
        }

        otherChar = GameObject.Instantiate<GameObject>(tmp);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void CreateGameObject()
    {
        GameObject tmp = Resources.Load<GameObject>("Cube");
        if (tmp != null)
        {
            otherChar = GameObject.Instantiate<GameObject>(tmp);
        }
        doCreate -= CreateGameObject;
    }

}
