using System.Net;
using System.Text;
using ServerCore;

namespace Server
{
    class Knight
    {
        public int _hp;
        public int _attak;

        public Knight(int hp, int attack) 
        {
            _hp = hp;
            _attak = attack;
        }
    }

    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Knight knight = new Knight(10, 100);

            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
            
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffer)
        {
            int processedLength = buffer.Count; // 또는 실제로 처리한 데이터의 길이
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, processedLength);
            Console.WriteLine($"[From Client] {recvData}");

            return processedLength;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }

    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            Console.WriteLine("Server shutting down...");
        }
    }
}
