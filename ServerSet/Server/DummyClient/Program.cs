using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    public class Packet
    {
        public ushort Size { get; set; }
        public ushort PacketId { get; set; }

        // 패킷을 바이트 배열로 직렬화
        public byte[] Serialize()
        {
            byte[] sizeBytes = BitConverter.GetBytes(Size);
            byte[] packetIdBytes = BitConverter.GetBytes(PacketId);

            // 직렬화된 바이트 배열을 생성
            byte[] result = new byte[sizeBytes.Length + packetIdBytes.Length];
            Array.Copy(sizeBytes, 0, result, 0, sizeBytes.Length);
            Array.Copy(packetIdBytes, 0, result, sizeBytes.Length, packetIdBytes.Length);

            return result;
        }

        // 바이트 배열로부터 패킷을 역직렬화
        public static Packet Deserialize(byte[] data)
        {
            if (data.Length < 4) // 최소한 4바이트 (2바이트 크기 + 2바이트 패킷 ID)
            {
                throw new ArgumentException("Data is too short to deserialize");
            }

            ushort size = BitConverter.ToUInt16(data, 0);
            ushort packetId = BitConverter.ToUInt16(data, 2);

            return new Packet { Size = size, PacketId = packetId };
        }
    }

    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Packet packet = new Packet
            {
                Size = 4,
                PacketId = 7
            };

            for (int i = 0; i < 5; i++)
            {
                byte[] serializedPacket = packet.Serialize();
                ArraySegment<byte> sendBuff = new ArraySegment<byte>(serializedPacket);

                Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffer)
        {
            int processedLength = buffer.Count;
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, processedLength);
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
            Console.WriteLine("<<<<<<<<<<<Client>>>>>>>>>>");
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return new GameSession(); });

            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            Console.WriteLine("\nClient shutting down...");
        }
    }
}
