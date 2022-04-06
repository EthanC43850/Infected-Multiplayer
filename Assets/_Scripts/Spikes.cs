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

        IDamageable player = other.gameObject.GetComponent<IDamageable>();
        if (player != null)
        {
            player.TakeDamage(5);
        }
    }


}
