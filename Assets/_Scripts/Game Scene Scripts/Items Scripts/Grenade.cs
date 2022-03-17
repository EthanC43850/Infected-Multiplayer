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
    public GameObject grenadePrefab;
    public PlayerController playerController;
    public DrawProjection drawProjectionScript;
    public Transform throwAngle;
    public Transform throwPos;
    public Rigidbody rb;

    public bool triggered = false;

    PhotonView PV;
    GameObject radiusObject;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

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

        
        //GameObject grenade = Instantiate(grenade, throwAngle.position, throwAngle.rotation);
        gameObject.transform.parent = null;
        drawProjectionScript.enabled = false;
        rb.isKinematic = false;
        rb.velocity = throwAngle.transform.up * throwDistance;

        if (true)   // If has more grenades
        {
            
            GameObject newGrenade = Instantiate(grenadePrefab, throwPos.position, Quaternion.identity, throwPos);
            playerController.items[playerController.currentItemIndex].itemGameObject = newGrenade;


        }

        //PV.RPC("RPC_ThrowProjectile", RpcTarget.All);
        

        
    }


    [PunRPC]
    void RPC_ThrowProjectile()
    {

        

    }


    public void OnCollisionEnter(Collision collision)
    {
        if (triggered == false)
        {
            StartCoroutine(DetonateCoroutine());
        }
        else
        {
            radiusIndicator.transform.position = transform.position;   // If radius collides with something else, move its position
        }

        triggered = true;


    }


    IEnumerator DetonateCoroutine()
    {
        radiusObject = Instantiate(radiusIndicator, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(((ExplosiveInfo)itemInfo).detonationTime);
        Destroy(radiusObject);
        GameObject explosion = Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
        yield return new WaitForSeconds(0.5f);
        Detonate();

    }


    void Detonate()
    {
        Vector3 explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, ((ExplosiveInfo)itemInfo).radius);

        foreach (Collider hit in colliders)
        {
            hit.gameObject.GetComponent<IDamageable>()?.TakeDamage(((ExplosiveInfo)itemInfo).damage);

        }

        Destroy(gameObject);



    }


}
