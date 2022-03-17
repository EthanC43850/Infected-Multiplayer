using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrowable : MonoBehaviour
{

    /*public ExplosiveInfo explosiveInfo;
    private GameObject radiusObject;
    bool triggered = false;


    public void OnCollisionEnter(Collision collision)
    {
        if(triggered == false)
        {
            StartCoroutine(DetonateCoroutine());
        }
        else
        {
            radiusObject.transform.position = transform.position;   // If radius collides with something else, move its position
        }

        triggered = true;


    }


    IEnumerator DetonateCoroutine()
    {
        radiusObject = Instantiate(explosiveInfo.radiusIndicator, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(explosiveInfo.detonationTime);
        Destroy(radiusObject);
        GameObject explosion = Instantiate(explosiveInfo.explosionParticle, transform.position, explosiveInfo.explosionParticle.transform.rotation);
        yield return new WaitForSeconds(0.5f);
        Detonate();

    }


    void Detonate()
    {
        Vector3 explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosiveInfo.radius);

        foreach (Collider hit in colliders)
        {
            hit.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)explosiveInfo).damage);

        }

        Destroy(gameObject);



    }*/

}
