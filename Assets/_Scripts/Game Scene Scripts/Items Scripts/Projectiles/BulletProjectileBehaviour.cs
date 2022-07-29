using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectileBehaviour : MonoBehaviour
{
    [HideInInspector] public GunInfo gunInfo;
    private Rigidbody bulletRigidBody;


    public void Init(GunInfo _gunInfo)
    {
        gunInfo = _gunInfo;
        bulletRigidBody = GetComponent<Rigidbody>();
        Destroy(gameObject, gunInfo.bulletLifeTime);

    }


    private void Update()
    {
        Vector3 movement = transform.forward * gunInfo.bulletSpeed * Time.deltaTime;
        bulletRigidBody.MovePosition(transform.position + movement);

    }


    private void OnTriggerEnter(Collider other)
    {

        WorldSpacePlayerUI worldSpaceUI = other.gameObject.GetComponentInChildren<WorldSpacePlayerUI>();

        other.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)gunInfo).damage);
        other.gameObject.GetComponentInChildren<WorldSpacePlayerUI>()?.DisplayFloatingText(((WeaponInfo)gunInfo).damage);


        /// Single Player Testing
        if (PlayerController.debugMode == true)
        {
            // Can instantiate different impact particles based on what the other object hit is
            Instantiate(gunInfo.hitWallParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }

    }


}
