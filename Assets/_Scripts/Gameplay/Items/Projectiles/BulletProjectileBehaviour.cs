using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BulletProjectileBehaviour : MonoBehaviour
{
    [HideInInspector] public GunInfo gunInfo;
    private Rigidbody bulletRigidBody;
    private PhotonMessageInfo bulletOwner;
    

    public void Init(GunInfo _gunInfo)
    {
        gunInfo = _gunInfo;
        bulletRigidBody = GetComponent<Rigidbody>();
        Destroy(gameObject, gunInfo.bulletLifeTime);

    } // END Init

    public void InitBulletOwner(PhotonMessageInfo _info)
    {
        bulletOwner = _info;

    }


    private void Update()
    {
        Vector3 movement = transform.forward * gunInfo.bulletSpeed * Time.deltaTime;
        bulletRigidBody.MovePosition(transform.position + movement);

    }


    private void OnTriggerEnter(Collider other)
    {

        // Can instantiate different impact particles based on what the other object hit is
        Instantiate(gunInfo.hitWallParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);


        // Using PV of others
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();

        Debug.Log("hit collider " + other.name);
        //Debug.Log(pv.name);

        if (pv != null)
        {
            Debug.Log("PV IS NOT NULL FOR " + other.name);

            if (!pv.IsMine) // Bullet hits player or AI Unit, pv = player/AI hit, !pv.IsMine = the local client
            {
                other.gameObject.GetComponentInChildren<WorldSpacePlayerUI>()?.DisplayFloatingText(((WeaponInfo)gunInfo).damage); // Display damage on all clients

            }
            else // Take damage on local client
            {
                other.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)gunInfo).damage);


            }
        }

        if (PlayerController.debugMode) 
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)gunInfo).damage);
            other.gameObject.GetComponentInChildren<WorldSpacePlayerUI>()?.DisplayFloatingText(((WeaponInfo)gunInfo).damage);

        }

    }


} // END Class
