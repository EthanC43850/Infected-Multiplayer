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

    [Header("Player Stats")]
    int kills;
    int deaths;
    public bool foundCure = false;

    [Header("Managers")]
    ZombieSpawnManager zombieSpawnManager;

    [Header("Additional Components")]
    PhotonView PV;
    GameObject controller;

    #endregion


    #region Monobehaviours

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
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID }); // PV.ViewID allows the controller to find its own playermanager.                                                                                                                                                                  //PV.ViewID is index 0 of the array of objects that were passed.
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

    } // END CreateController


    //-------------------------------------------//
    public void Die()
    {
        foundCure = false;

        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

    }

    //-------------------------------------------//
    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }


    //-------------------------------------------//
    [PunRPC]
    public void RPC_GetKill()
    {

        Debug.Log("manager Called GETKILL" + gameObject.name);

        kills++;
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        // Turned Gamemode 
        if (!foundCure && SceneManager.GetActiveScene().buildIndex == 1)
        {
            RespawnAsSurvivor();
        }
    } 


    //-------------------------------------------//
    public void RespawnAsSurvivor()     // Respawn Zombie as a Survivor
    {
        foundCure = true;
        PhotonNetwork.Destroy(controller);
        CreateController();
    } 



    //-------------------------------------------//
    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player); // Find Player manager associated with message
    }


    #endregion

} // END Class
