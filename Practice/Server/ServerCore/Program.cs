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

                // 받는다
                byte[] recvBuffer = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuffer);
                string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);
                Console.WriteLine($"[From Client] : {recvData}");

                // 보낸다
                byte[] sendBuffer = Encoding.UTF8.GetBytes(greetings);
                clientSocket.Send(sendBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Main(string[] args)
        {
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