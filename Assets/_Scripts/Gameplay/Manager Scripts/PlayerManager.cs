using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{

    PhotonView PV;
    GameObject controller;

    //public delegate void AddToAIEnemyLists(Targetable target);
    //public static event AddToAIEnemyLists AddEnemyToAILists;

    private void Awake()
    {
        PhotonNetwork.RunRpcCoroutines = true;
        PV = GetComponent<PhotonView>();

    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }

    }

    public void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID }); // Is punRPC and network.instantiate the same?
                                                                                                                                                                            // No, Network.instantiate properly assigns a PhotonView to the object

        Debug.Log("ABOUT TO ADD PLAYER TO TARGET LIST 1");

        PV.RPC("AddPlayersToTargetLists", RpcTarget.All);

    }


    [PunRPC]
    public void AddPlayersToTargetLists()
    {
        Debug.Log("ABOUT TO ADD PLAYER TO TARGET LIST 2");
        ZombieSpawnManager.Instance.targets.Add(controller.GetComponent<Targetable>());   // This could be an issue when loading different levels

    }




    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

    }




} // END Class
