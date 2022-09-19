using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{

    #region Variables

    [Header("Projectile Settings")]
    public Transform bulletSpawnPoint;
    /*public Transform rightHandTransform;
    public Transform rightHandLocalTransformValues;*/

    [Header("Particles")]
    public ParticleSystem spawnParticles;

    [Header("Audio")]
    public AudioSource spawnAudioSource;



    private float timer;

    PhotonView PV;


    #endregion


    #region Monobehaviours
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        // Attempted to animate gun with right hand, when running backwards the gun actually gets aimed up
        /*if(rightHandTransform != null)
        {
            transform.SetParent(rightHandTransform, true);
            transform.position = rightHandLocalTransformValues.position;
            transform.rotation = rightHandLocalTransformValues.rotation;
            transform.localScale = rightHandLocalTransformValues.localScale;
        }*/

    }


    public void Update()
    {
        timer += Time.deltaTime;
    }

    #endregion


    #region Methods
    public override void Use()
    {
        Debug.Log("Firing " + itemGameObject.name);
        if(timer >= ((GunInfo)itemInfo).shootRate)
        {
            Shoot();
        }

    } // END Use


    public void Shoot()
    {

        timer = 0f;

        if(PlayerController.debugMode == false)
        {
            PV.RPC(nameof(RPC_Shoot), RpcTarget.All, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
        else
        {
            BulletProjectileBehaviour _bulletInfo = Instantiate(((GunInfo)itemInfo).bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<BulletProjectileBehaviour>();
            _bulletInfo.Init((GunInfo)itemInfo);


            if (spawnParticles)
            {
                spawnParticles.Play();
            }

            if (spawnAudioSource)
            {
                spawnAudioSource.Play();
            }
        }
        



    } // END Shoot


    [PunRPC]
    void RPC_Shoot(Vector3 initialPosition, Quaternion initialRotation, PhotonMessageInfo info)
    {
        BulletProjectileBehaviour _bulletInfo = Instantiate(((GunInfo)itemInfo).bulletPrefab, initialPosition, initialRotation).GetComponent<BulletProjectileBehaviour>();
        _bulletInfo.Init((GunInfo)itemInfo);   

        // Helps keep track of kills and avoids having bullets collide with the shooter
        _bulletInfo.InitBulletOwner(PV, gameObject.GetComponentInParent<PlayerController>().faction);

        if (spawnParticles)
        {
            spawnParticles.Play();
        }

        if (spawnAudioSource)
        {
            spawnAudioSource.Play();
        }

    } // END RPC_Shoot


    public override void FinishedUse()
    {
        throw new System.NotImplementedException();
    }

    #endregion

} // END Class
