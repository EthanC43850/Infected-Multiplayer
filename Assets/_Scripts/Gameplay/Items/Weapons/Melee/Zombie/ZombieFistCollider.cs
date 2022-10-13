using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ZombieFistCollider : MonoBehaviour
{
    // This could be melee functionality for anything
    // Soon I will change zombie controller script to targetable
    public ZombieController zombieControllerScript;

    // Need to create a melee type of item instead of zombie and create abstract functinality
    public ZombieFists zombieFistsScript;

    Rigidbody playerRigidbody;

    private void Awake()
    {
        playerRigidbody = gameObject.GetComponentInParent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!zombieFistsScript.PV.IsMine && PlayerController.debugMode == false) { return; }

        Targetable target = other.gameObject.GetComponent<Targetable>();

        if (target == null) { return; }

        //Debug.Log("trigger detected " + other.name);

        if(target.faction == zombieControllerScript.faction) // Avoid hitting self, as well as teammates
        {
            return;

        }
        else
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(((WeaponInfo)zombieFistsScript.itemInfo).damage);

            if (target.isDead)
            {
                Debug.Log("Killed target!");

            }
            else
            {
                //Debug.Log("Did not kill target");

            }

            WorldSpacePlayerUI worldSpaceUI = other.gameObject.GetComponentInChildren<WorldSpacePlayerUI>();
            if (worldSpaceUI != null)
            {
                worldSpaceUI.DisplayFloatingText(((WeaponInfo)zombieFistsScript.itemInfo).damage);

            }

            this.gameObject.SetActive(false); // Allow only each hand to hit once

            // Add knockback force relative to player
            Vector3 moveDirection = playerRigidbody.transform.position - other.transform.position;
            playerRigidbody.AddForce(moveDirection.normalized * -500f);


        }
        
    }



} // END Class
