using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{

    #region Variables

    [Header("Projectile Settings")]
    public Transform bulletSpawnPoint;

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
        if(timer >= ((GunInfo)itemInfo).shootRate)
        {
            Shoot();

        }

    } // END Use


    public void Shoot()
    {

        timer = 0f;
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


    } // END Shoot


    public override void FinishedUse()
    {
        throw new System.NotImplementedException();
    }

    #endregion

} // END Class
