using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BulletProjectileBehaviour : MonoBehaviour
{
    [HideInInspector] public GunInfo gunInfo;
    private Rigidbody bulletRigidBody;
    public PhotonView PV;
    GameObject shooterPlayer;

    public void Init(GunInfo _gunInfo)
    {
        gunInfo = _gunInfo;
        bulletRigidBody = GetComponent<Rigidbody>();
        Destroy(gameObject, gunInfo.bulletLifeTime);

    } // END Init

    public void InitBulletOwner(PhotonView _PV, GameObject _shooterPlayer) 
    {
        // Track which bullets belong to which player
        PV = _PV;
        shooterPlayer = _shooterPlayer;
    }


    private void Update()
    {
        Vector3 movement = transform.forward * gunInfo.bulletSpeed * Time.deltaTime;
        bulletRigidBody.MovePosition(transform.position + movement);

    }


    private void OnTriggerEnter(Collider other)
    {
        // Avoid having bullet collide with shooter himself
        if (other.gameObject == shooterPlayer)
        {
            return;
        }

        // Can instantiate different impact particles based on what object was hit
        Instantiate(gunInfo.hitWallParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);


        Debug.Log("hit collider " + other.name + " SHOOTER OBJECT IS: " + shooterPlayer.name);
        //Debug.Log(pv.name);

        if (PV != null)
        {
            //Debug.Log("PV IS NOT NULL FOR " + other.name);

            // Bullet hits player or AI Unit, pv = player/AI hit, !pv.IsMine = the local client
            if (PV.IsMine) 
            {
                other.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)gunInfo).damage);
            }
            else 
            {
                // Display damage on local client
                other.gameObject.GetComponentInChildren<WorldSpacePlayerUI>()?.DisplayFloatingText(((WeaponInfo)gunInfo).damage); // Display damage on all clients
            }
        }

        if (PlayerController.debugMode) 
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)gunInfo).damage);
            other.gameObject.GetComponentInChildren<WorldSpacePlayerUI>()?.DisplayFloatingText(((WeaponInfo)gunInfo).damage);

        }

    }


} // END Class
