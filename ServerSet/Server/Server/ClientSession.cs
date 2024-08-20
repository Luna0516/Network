using ServerCore;
using System.Net;

namespace Server
{
    // 최대한 압축하자!
    public class Packet
    {
        public ushort Size { get; set; }
        public ushort PacketId { get; set; }
    }

    public class PlayerInfoReq : Packet
    {
        public long PlayerId { get; set; }
    }

    public class PlayerInfoOk : Packet
    {
        public int Hp { get; set; }
        public int Attack { get; set; }
    }

    public enum PacketID
    {
        PlayerInforReq = 1,
        PlayerInfoOk = 2,
    }

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnReceivePacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;

            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInforReq:
                    long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                    count += 8;
                    Console.WriteLine($"PlayerInforReq : {playerId}");
                    break;
                case PacketID.PlayerInfoOk:
                    break;
                default:
                    break;
            }

            Console.WriteLine($"Receive PacketId : {id}, Size : {size}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }


}
