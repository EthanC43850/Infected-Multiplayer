using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Spikes : MonoBehaviourPunCallbacks
{

    PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (!player.PV.IsMine) { return; }

        if (player != null)
        {
            player.TakeDamage(20);
        }
    }


}
