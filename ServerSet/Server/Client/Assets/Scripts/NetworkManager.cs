using DummyClient;
using ServerCore;
using System;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    private void Start()
    {
        Console.WriteLine("<<<<<<<<<<<Client>>>>>>>>>>");
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();
        connector.Connect(endPoint,
            () => { return _session; },
            1);
    }
}
