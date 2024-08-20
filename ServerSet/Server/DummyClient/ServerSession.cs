using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    public class Packet
    {
        public ushort Size { get; set; }

        public ushort PacketId { get; set; }
    }

    public enum PacketID
    {
        PlayerInforReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint} {(ushort)4}");

            Packet packet = new Packet
            {
                Size = 4,
                PacketId = (ushort)PacketID.PlayerInforReq
            };

            for (int i = 0; i < 3; i++)
            {
                ArraySegment<byte> s = SendBufferHelper.Open(4096);
                byte[] size = BitConverter.GetBytes(packet.Size);
                byte[] packetId = BitConverter.GetBytes(packet.PacketId);

                Array.Copy(size, 0, s.Array, s.Offset, size.Length);
                Array.Copy(packetId, 0, s.Array, s.Offset + size.Length, packetId.Length);

                ArraySegment<byte> c = SendBufferHelper.Close(packet.Size);
                Send(c);
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
}
