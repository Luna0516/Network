using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client_B
{
    class Program
    {
        static string _greetings = "Hello! I am Client_B";

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipaddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipaddr, 8000);

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 입장문의
                socket.Connect(endPoint);
                Console.WriteLine($"Connected To {socket.RemoteEndPoint?.ToString()}");

                // 보낸다
                byte[] sendBuffer = Encoding.UTF8.GetBytes(_greetings);
                socket.Send(sendBuffer);

                // 받는다
                byte[] recvBuffer = new byte[1024];
                int bytes = socket.Receive(recvBuffer);
                string data = Encoding.UTF8.GetString(recvBuffer, 0, bytes);
                Console.WriteLine($"[From Client] : {data}");

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
