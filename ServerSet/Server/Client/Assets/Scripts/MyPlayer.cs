using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;

    private void Start()
    {
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if (_network == null)
        {
            Debug.LogWarning("Not Find NetworkManager");
            return;
        }

        StartCoroutine(CoSendPacket());
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            C_Move movePacket = new C_Move();
            movePacket.posX = Random.Range(-50.0f, 50.0f);
            movePacket.posY = 0.0f;
            movePacket.posZ = Random.Range(-50.0f, 50.0f);
            _network.Send(movePacket.Write());
        }
    }
}
