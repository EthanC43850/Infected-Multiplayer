using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieFists : Item
{

    #region Variables

    public GameObject[] damageColliders;
    public Animator animator;
    public PhotonView PV;

    public bool isAttacking = false;

    #endregion


    public override void Use()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        /*while(isAttacking)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) // Avoid repeating attack on button click
            {
                continue;
            }
            //animator.SetTrigger("Attack"); // Damage dealt through animation events


        }*/

    }

    public void EnableDamageColliders()
    {
        foreach (GameObject collider in damageColliders)
        {
            collider.SetActive(true);
        }
    }

    public void DisableDamageColliders()
    {
        foreach (GameObject collider in damageColliders)
        {
            collider.SetActive(false);
        }
    }

    public override void FinishedUse()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);


    }
} // END Class
