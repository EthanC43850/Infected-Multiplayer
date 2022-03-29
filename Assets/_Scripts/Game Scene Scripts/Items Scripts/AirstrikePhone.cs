using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirstrikePhone : Explosive
{

    [Header("Airstrike Properties")]
    public float airStrikeIndicatorMoveSpeed;

    [Header("Object Connections")]
    
    public GameObject missileObject;
    public GameObject airStrikeIndicator;   // This is also located in explosive info which i have no idea how to type cast
    public PhotonView PV;
    

    public override void Use()
    {
        SummonAirstrike();
    }

    private void SummonAirstrike()
    {

        if (PlayerController.debugMode == false)
        {
            PV.RPC("RPC_ThrowProjectile", RpcTarget.All);

        }
        else
        {
            GameObject airStrike = Instantiate(missileObject, airStrikeIndicator.transform.position, airStrikeIndicator.transform.rotation);
            
        }


    }


    [PunRPC]
    void RPC_ThrowProjectile()
    {
        GameObject missile = Instantiate(missileObject, airStrikeIndicator.transform.position, airStrikeIndicator.transform.rotation);


        //grenade.GetComponent<Rigidbody>().velocity = ShotPoint.transform.up * throwDistance;


    }

}
