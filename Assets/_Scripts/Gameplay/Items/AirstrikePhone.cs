using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirstrikePhone : Explosive
{

    [Header("Airstrike Properties")]
    public float indicatorMoveSpeed;

    [Header("Object Connections")]
    public GameObject missileObject;
    public GameObject airStrikeIndicator; // Attached to phone weapon
    public PhotonView PV;

    private Vector3 missileHeightOffset = new Vector3(0f, 20f, 0f);
    private ExplosiveInfo explosiveInfo;

    private void OnEnable()
    {
        explosiveInfo = (ExplosiveInfo)itemInfo;
        airStrikeIndicator.transform.localScale += new Vector3(explosiveInfo.radius + explosiveInfo.radiusIndicatorOffset, explosiveInfo.radius + explosiveInfo.radiusIndicatorOffset, explosiveInfo.radius + explosiveInfo.radiusIndicatorOffset);
        PlayerController player = gameObject.GetComponentInParent<PlayerController>();

        if(player != null)
        {
            player.airstrikePhone = this;
        }

    }


    public override void Use()
    {
        SummonAirstrike();
    }

    private void SummonAirstrike()
    {

        if (PlayerController.debugMode == false)
        {
            PV.RPC("RPC_LaunchAirstrike", RpcTarget.All);
             
        }
        else
        {

            AirstrikeMissile airStrike = Instantiate(missileObject, airStrikeIndicator.transform.position + missileHeightOffset, Quaternion.identity).GetComponent<AirstrikeMissile>();
            airStrike.InstantiateIndicator(airStrikeIndicator.transform);
        }


    }


    // AIRSTRIKES CURRENTLY DO DOUBLE DAMAGE BECAUSE THE INSTANTIATED OBJECTS RUN LOCALLY.
    // FIX: Implement Detonation function INSIDE the PunRPC just like in the "SingleShotGun" Scripts


    [PunRPC]
    void RPC_LaunchAirstrike()
    {
        AirstrikeMissile airStrike = Instantiate(missileObject, airStrikeIndicator.transform.position + missileHeightOffset, Quaternion.identity).GetComponent<AirstrikeMissile>();
        airStrike.InstantiateIndicator(airStrikeIndicator.transform);


    }

    public override void FinishedUse()
    {
        throw new NotImplementedException();
    }
}
