using ServerCore;

namespace Server
{
    class PacketHandler
    {
        public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
        {
            PlayerInfoReq p = packet as PlayerInfoReq;

            Console.WriteLine($"PlayerInforReq : {p.playerId}, Name : {p.name}");

            Console.WriteLine($"Skill : (Id), (Level), (Duration)");
            foreach (PlayerInfoReq.Skill skill in p.skills)
            {
                Console.WriteLine($"Skill : ({skill.id}), ({skill.level}), ({skill.duration})");
            }
        }
    }
}
