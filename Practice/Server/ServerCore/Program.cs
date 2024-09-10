using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static int _listener = 10;

        static string _greetings = "Welcome to MMORPG Server!";

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipaddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipaddr, 8000);

            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {

                listenSocket.Bind(endPoint);
                listenSocket.Listen(_listener);

                while (true)
                {
                    Console.WriteLine("Listening...");

                    Socket clientSocket = listenSocket.Accept();
                    // 받는다
                    byte[] recvbuffer = new byte[1024];
                    int bytes = clientSocket.Receive(recvbuffer);
                    string data = Encoding.UTF8.GetString(recvbuffer, 0, bytes);
                    Console.WriteLine($"[From Client] : {data}");

                    // 보낸다
                    byte[] sendBuffer = Encoding.UTF8.GetBytes(_greetings);
                    clientSocket.Send(sendBuffer);

                    // 내보낸다
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}