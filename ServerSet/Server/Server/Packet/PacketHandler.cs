using ServerCore;

class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;

        Console.WriteLine($"PlayerInforReq : {p.playerId}, Name : {p.name}");

        Console.WriteLine($"Skill : (Id), (Level), (Duration)");
        foreach (C_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill : ({skill.id}), ({skill.level}), ({skill.duration})");
        }
    }
}