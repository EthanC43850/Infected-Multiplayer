using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class CurePlayer : MonoBehaviour
{
    PhotonView pv;
    Collider collider;
    GameObject meshPieces;

    private void Awake()
    {
        pv = gameObject.GetComponent<PhotonView>();
        collider = gameObject.GetComponent<Collider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player == null) { return; }
        
        if (player.PV.IsMine)
        {
            player.playerManager.RespawnAsSurvivor();
            pv.RPC(nameof(RPC_DestroyCure), RpcTarget.All);
        }
    }


    //-------------------------------------------//
    [PunRPC]
    public void RPC_DestroyCure()
    {
        collider.enabled = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        Debug.Log("Collided with cure");
    }


} // END Class
