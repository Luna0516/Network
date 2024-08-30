using DummyClient;
using ServerCore;
using UnityEngine;

class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        // if (chatPacket.playerId == 1)
        { 
            Debug.Log(chatPacket.chat);

            // 유니티 코드 접근 못함!
            GameObject go = GameObject.Find("Player");
            if (go == null)
                Debug.Log("Player Not Found");
            else
                Debug.Log("Player Found");
        }
    }
}