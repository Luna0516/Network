using System.Net;
using ServerCore;

namespace Server
{
    // 최대한 압축하자!
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

    class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnReceivePacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort Id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"ReceivePacketId : {Id}, Size : {size}");
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
            Console.WriteLine("<<<<<<<<<<<Server>>>>>>>>>>");
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

            Console.WriteLine("\nServer shutting down...");
        }
    }
}
