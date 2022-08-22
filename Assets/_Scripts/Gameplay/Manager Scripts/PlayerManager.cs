using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    PhotonView PV;
    GameObject controller;

    ZombieSpawnManager zombieSpawner;

    int kills;

    public bool foundCure = false;

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

    #endregion

    #region Methods



    //-------------------------------------------//
    public void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();


        if (SceneManager.GetActiveScene().buildIndex == 1) // Turned GameMode
        {

            if (foundCure)
            {
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });

            }
            else
            {
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ZombieController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });

            }

        }
        else if (SceneManager.GetActiveScene().buildIndex == 2) // Survivor Vs Survivor GameMode
        {
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });


        }
        else
        {
            Debug.Log("Ethan - NO CONTROLLERS CREATED, WRONG SCENE INDEX");


        }




        // Is punRPC and network.instantiate the same?
        // No, Network.instantiate properly assigns a PhotonView to the object

        //PV.RPC("AddPlayersToTargetLists", RpcTarget.All); // Test for adding players to zombie spawner list
        //StartCoroutine(IWaitForObjects());

    }


    //-------------------------------------------//
    public void CreateZombieController()
    {

    }


    //-------------------------------------------//
    public void CreateSurvivorController()
    {




    }


    [PunRPC]
    public void AddPlayersToTargetLists()
    {
        Debug.Log("ABOUT TO ADD PLAYER TO TARGET LIST AND IS PHOTON VIEW MINE? : " + PV.IsMine);
        if (!PV.IsMine) { return; }
        ZombieSpawnManager.Instance.AddTargetToList(controller.GetComponent<Targetable>());   // This could be an issue when loading different levels
        //zombieSpawner.AddTargetToList(controller.GetComponent<Targetable>());

    }


    //-------------------------------------------//
    IEnumerator IWaitForObjects()
    {
        zombieSpawner = FindObjectOfType<ZombieSpawnManager>();
        yield return new WaitUntil(() => zombieSpawner != null);


    }


    //-------------------------------------------//
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

    }

    //-------------------------------------------//
    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);

    }


    //-------------------------------------------//
    public void RPC_GetKill()
    {
        kills++;
        Debug.Log("GOT A KILL");

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        if (!foundCure && SceneManager.GetActiveScene().buildIndex == 1)    // If playing turned Gamemode
        {
            foundCure = true;
            controller.gameObject.GetComponent<PlayerController>().Die();
        }

    } // END RPC_GetKill


    //-------------------------------------------//
    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player); // Not optimial, fix later
    }


    #endregion

} // END Class
