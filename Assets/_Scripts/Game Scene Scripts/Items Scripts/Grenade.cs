using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Item
{
    //[SerializeField] GameObject grenadePrefab;
    [SerializeField] GameObject explosionFX;

    public float throwDistance = 10;

    public float minThrowDistance;
    public float maxThrowDistance;

    [Header("Connections")]
    public DrawProjection drawProjectionScript;
    public GameObject grenadeThrowableObject;
    public Transform ShotPoint;

    //public GameObject Explosion;

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
        GameObject grenade = Instantiate(grenadeThrowableObject, ShotPoint.position, ShotPoint.rotation);
        grenade.GetComponent<Rigidbody>().velocity = ShotPoint.transform.up * throwDistance;



        
    }


}
