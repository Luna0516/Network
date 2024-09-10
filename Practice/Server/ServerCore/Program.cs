using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                string greetings = "Welcome to MMORPG Server!";

                Session session = new Session();
                session.Init(clientSocket);

                // 보낸다
                byte[] sendBuffer = Encoding.UTF8.GetBytes(greetings);
                session.Send(sendBuffer);

                Thread.Sleep(1000);

                session.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("<========== Server Core ==========>");

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");

            while (true)
            {
                Thread.Sleep(10);
            }
        }
    }
}