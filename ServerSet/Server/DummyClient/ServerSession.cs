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

    class ServerSession : Session
    {
        //static unsafe void ToBytes(byte[] array, int offset, ulong value)
        //{
        //    fixed (byte* ptr = &array[offset])
        //        *(ulong*)ptr = value;
        //}

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq
            {
                Size = 4,
                PacketId = (ushort)PacketID.PlayerInforReq,
                PlayerId = 1001
            };

            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            // success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.Size);
            count += 2;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.PacketId);
            count += 2;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.PlayerId);
            count += 8;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            ArraySegment<byte> c = SendBufferHelper.Close(count);

            if (success)
                Send(c);

            //byte[] size = BitConverter.GetBytes(packet.Size);
            //byte[] packetId = BitConverter.GetBytes(packet.PacketId);
            //byte[] playerId = BitConverter.GetBytes(packet.PlayerId);


            //Array.Copy(size, 0, s.Array, s.Offset + count, 2);
            //count += 2;

            //Array.Copy(packetId, 0, s.Array, s.Offset + count, 2);
            //count += 2;

            //Array.Copy(playerId, 0, s.Array, s.Offset + count, 8);
            //count += 8;

            //ArraySegment<byte> c = SendBufferHelper.Close(count);
            //Send(c);
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
