using ServerCore;
using System.Net;
using System.Text;

namespace Server
{
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
                case PacketID.PlayerInfoReq:
                    PlayerInfoReq p = new PlayerInfoReq();
                    p.Read(buffer);
                    Console.WriteLine($"PlayerInforReq : {p.playerId}, Name : {p.name}");
                    Console.WriteLine($"Skill : (Id), (Level), (Duration)");
                    foreach (PlayerInfoReq.Skill skill in p.skills)
                        Console.WriteLine($"Skill : ({skill.id}), ({skill.level}), ({skill.duration})");
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
