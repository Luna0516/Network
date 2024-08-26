using ServerCore;
using System.Net;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("<<<<<<<<<<<Client>>>>>>>>>>");
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, 
                () => { return SessionManager.Instance.Generate(); }, 
                500);

            while (true)
            {
                try
                {
                    SessionManager.Instance.SendForEach(); 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(250);

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            Console.WriteLine("\nClient shutting down...");
        }
    }
}
