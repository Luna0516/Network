using ServerCore;
using System.Net;
using System.Text;

namespace Client_B
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            // 보낸다
            string greetings = "Hello! I am Client_B";
            byte[] sendBuffer = Encoding.UTF8.GetBytes(greetings);
            Send(sendBuffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("<========== Client_B ==========>");

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            GameSession session = new GameSession();

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return session; });

            while (true)
            {
                Thread.Sleep(10);

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    session.Disconnect();
                    break;
                }
            }
        }
    }
}
