using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Explosive")]
public class ExplosiveInfo : WeaponInfo
{

    [Header("Explosive Properties")]
    public float radius = 2f;
    [Tooltip("The size of the indicator and overlap sphere function do not scale so you must adjust size to match")]
    public float radiusIndicatorOffset = 1f;
    public float detonationTime = 2f;

    [Header("Attributes")]
    public GameObject explosionParticle;
    public GameObject radiusIndicator;

} // END Class
