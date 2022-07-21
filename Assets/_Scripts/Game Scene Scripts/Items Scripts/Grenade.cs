using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Explosive
{
    public float throwDistance = 10;

    public float minThrowDistance;
    public float maxThrowDistance;

    [Header("Connections")]
    public DrawProjection drawProjectionScript;
    public GameObject grenadeThrowableObject;
    public Transform ShotPoint;

    public PhotonView PV;


    private void OnEnable()
    {
        drawProjectionScript.lineRenderer.enabled = false;
    }

    public override void Use()
    {
        ThrowProjectile();
    }


    public void ThrowProjectile()
    {
        

        if(PlayerController.debugMode == false)
        {
            PV.RPC("RPC_ThrowProjectile", RpcTarget.All);

        }
        else
        {
            GameObject grenade = Instantiate(grenadeThrowableObject, ShotPoint.position, ShotPoint.rotation);
            grenade.GetComponent<Rigidbody>().velocity = ShotPoint.transform.up * throwDistance;
        }

    }


    [PunRPC]
    void RPC_ThrowProjectile()
    {
        GameObject grenade = Instantiate(grenadeThrowableObject, ShotPoint.position, ShotPoint.rotation);
        grenade.GetComponent<Rigidbody>().velocity = ShotPoint.transform.up * throwDistance;


    }

    public override void FinishedUse()
    {
        throw new System.NotImplementedException();
    }
}
