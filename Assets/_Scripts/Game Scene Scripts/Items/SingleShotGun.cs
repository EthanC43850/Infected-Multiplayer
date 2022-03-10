using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{

    #region Variables

    [SerializeField] Transform gunBarrel;
    PhotonView PV;


    #endregion


    #region Monobehaviours
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void Update()
    {
        Debug.DrawRay(gunBarrel.position, gunBarrel.transform.forward * 4, Color.red);
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
        if(Physics.Raycast(gunBarrel.position, gunBarrel.transform.forward, out RaycastHit hit, 4.0f))
        {
            PlayerController player = hit.collider.gameObject.GetComponent<PlayerController>();
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);

            if(player != null)
            {
                player.worldSpaceUI.DisplayFloatingText(((GunInfo)itemInfo).damage);
            }

            Instantiate(bulletImpactPrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal, Vector3.up) * bulletImpactPrefab.transform.rotation);

            //PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            
        }

    } // END Shoot


    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
        // add photon view to bullet impacts, <3 you future E keep goin

    } // RPC_Shoot

    #endregion

}
