using Server;
using ServerCore;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        C_LeaveGame leavePacket = packet as C_LeaveGame;
        ClientSession clientSession = session as ClientSession;
        ;
        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;
        ;
        if (clientSession.Room == null)
            return;

        Console.WriteLine($"Vector3({movePacket.posX}, {movePacket.posY}, {movePacket.posZ})");

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }
}