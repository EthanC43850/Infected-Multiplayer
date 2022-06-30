using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Spikes : MonoBehaviourPunCallbacks
{

    PhotonView pv;
    public GameObject zombieTestDelMe;
    public Transform zombieSpawnPoint;

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

        // This is null for some reason. Why???
        zombieTestDelMe = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ZombieUnit"), zombieSpawnPoint.position, zombieSpawnPoint.rotation, 0, new object[] { pv.ViewID });




    }


}
