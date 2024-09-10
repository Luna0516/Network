using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client_B
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("<========== Client_B ==========>");

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 입장문의
                socket.Connect(endPoint);
                Console.WriteLine($"Connected To {socket.RemoteEndPoint?.ToString()}");

                // 보낸다
                string greetings = "Hello! I am Client_B";
                byte[] sendBuffer = Encoding.UTF8.GetBytes(greetings);
                socket.Send(sendBuffer);

                // 받는다
                byte[] recvBuffer = new byte[1024];
                int recvBytes = socket.Receive(recvBuffer);
                if (recvBytes > 0)
                {
                    string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");
                }
                else
                {
                    Console.WriteLine("Failed to receive data from the server.");
                }

                // 나간다
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
