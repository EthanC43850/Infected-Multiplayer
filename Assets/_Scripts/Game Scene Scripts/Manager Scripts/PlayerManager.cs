using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{

    PhotonView PV;
    GameObject controller;

    public delegate void AddToAIEnemyLists(Targetable target);
    public static event AddToAIEnemyLists AddEnemyToAILists;

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
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        AddEnemyToAILists.Invoke(controller.GetComponent<Targetable>());
        Debug.Log(controller.GetComponent<Targetable>().name);
    }


    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }




}
