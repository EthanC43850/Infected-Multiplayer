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
        if(player == null) { return; }
        if (!player.PV.IsMine) { return; }


        if (player != null)
        {
            player.TakeDamage(5);
        }

        // returns null, WHY??
        //zombieTestDelMe = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ZombieUnit"), zombieSpawnPoint.position, zombieSpawnPoint.rotation, 0, new object[] { pv.ViewID });

        if (PlayerController.debugMode)
        {
            Instantiate(zombieTestDelMe, zombieSpawnPoint.transform.position, Quaternion.identity);

        }
        else
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Zombie_AI ( Non-Controllable)"), zombieSpawnPoint.position, zombieSpawnPoint.rotation, 0, new object[] { pv.ViewID });



        }



    }

}
