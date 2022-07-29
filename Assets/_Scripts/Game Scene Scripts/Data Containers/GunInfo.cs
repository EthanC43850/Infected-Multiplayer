using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : WeaponInfo
{
    public float shootRate;
    public float bulletSpeed;
    public float bulletLifeTime;

    public GameObject bulletPrefab;
    public GameObject hitEnemyParticles;
    public GameObject hitWallParticles;


}
