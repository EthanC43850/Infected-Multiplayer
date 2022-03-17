using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Explosive")]
public class ExplosiveInfo : WeaponInfo
{
    [Header("Explosive Properties")]
    public float radius = 2f;
    public float detonationTime = 2f;

    [Header("Attributes")]
    public GameObject explosionParticle;
    public GameObject radiusIndicator;

}
