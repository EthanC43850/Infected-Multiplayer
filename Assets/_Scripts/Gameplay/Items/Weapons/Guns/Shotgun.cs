using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{

    #region Variables

    [Header("Projectile Settings")]
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] GameObject projectilePrefab;

    [SerializeField] float shootRate;
    private float timer;

    [Header("Particles")]
    [SerializeField] ParticleSystem spawnParticles;

    [Header("Audio")]
    [SerializeField] AudioSource spawnAudioSource;



    PhotonView PV;


    #endregion
     

    #region Monobehaviours
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void Update()
    {
        timer += Time.deltaTime;
        //Debug.DrawRay(bulletSpawnPoint.position, bulletSpawnPoint.transform.forward * 4, Color.red);
    }

    #endregion


    #region Methods
    public override void Use()
    {
        Debug.Log("Firing " + itemGameObject.name);
        Shoot();

    } // END Use

    
    public void Shoot()
    {

        /*timer = 0f;
        Instantiate(projectilePrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        if (spawnParticles)
        {
            spawnParticles.Play();
        }

        if (spawnAudioSource)
        {
            spawnAudioSource.Play();
        }
        */


        #region Shooting Method Through Raycast (Could be useful for fast shortranged shotguns)


        if (Physics.Raycast(bulletSpawnPoint.position, bulletSpawnPoint.transform.forward, out RaycastHit hit, 4.0f))
        {
            WorldSpacePlayerUI worldSpaceUI = hit.collider.gameObject.GetComponentInChildren<WorldSpacePlayerUI>();
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)itemInfo).damage);

            if(worldSpaceUI != null)
            {
                worldSpaceUI.DisplayFloatingText(((WeaponInfo)itemInfo).damage);

            }

            /// Single Player Testing
            if (PlayerController.debugMode == true)
            {
               // Instantiate(bulletImpactPrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal, Vector3.up) * bulletImpactPrefab.transform.rotation);

                Collider[] colliders = Physics.OverlapSphere(hit.point, 0.3f);
                if (colliders.Length != 0)
                {
                    //GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal, Vector3.up) * bulletImpactPrefab.transform.rotation);
                    //Destroy(bulletImpactObj, 10f);
                    //bulletImpactObj.transform.SetParent(colliders[0].transform);
                }
            }
            else
            {
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);

            }


        }

        #endregion


    } // END Shoot


    // Would need to implement object pooling through a network
    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        // Just a bullet hole effect

        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
           // GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
           // Destroy(bulletImpactObj, 10f);
           // bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
        // <3 you future E keep goin

    } // RPC_Shoot

    public override void FinishedUse()
    {
        throw new System.NotImplementedException();
    }

    #endregion

} // END Class
