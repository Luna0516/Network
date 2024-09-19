using ServerCore;
using System.Net;
using System.Text;

namespace Client_A
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            //string greetings = "Hello! I am Client_A";
            //byte[] sendData = Encoding.UTF8.GetBytes(greetings);
            //ArraySegment<byte> sendBuffer = new ArraySegment<byte>(sendData);
            //Send(sendBuffer);

            Packet packet = new Packet
            {
                Size = 4,
                PacketId = 7
            };
            Packet packet2 = new Packet
            {
                Size = 4,
                PacketId = 3
            };

            byte[] serializedPacket = packet.Serialize();
            ArraySegment<byte> sendBuffer2 = new ArraySegment<byte>(serializedPacket);
            Send(sendBuffer2);

            byte[] serializedPacket2 = packet2.Serialize();
            ArraySegment<byte> sendBuffer3 = new ArraySegment<byte>(serializedPacket2);
            Send(sendBuffer3);
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
            Console.WriteLine("<========== Client_A ==========>");

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