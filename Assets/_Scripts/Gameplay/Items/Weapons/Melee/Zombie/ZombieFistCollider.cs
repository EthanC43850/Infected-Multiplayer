using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ZombieFistCollider : MonoBehaviour
{
    public ZombieController zombieControllerScript;
    public ZombieFists zombieFistsScript;

    void OnTriggerEnter(Collider other)
    {
        if (!zombieFistsScript.PV.IsMine && PlayerController.debugMode == false) { return; }

        Targetable target = other.gameObject.GetComponent<Targetable>();

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
        }
        
    }



} // END Class
