using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class Zombie_Unit : AgentStateMachine
{
    #region Variables


    [Header("Connections")]
    [SerializeField] Transform damageOutputPoint;

    #endregion


    #region Monobehaviours





    #endregion


    #region Methods

    //-------------------------------------------//
    public override void DealBlow()
    {
        if (Time.time >= lastBlowTime + attackRatio)
        {
            lastBlowTime = Time.time;

            //Debug.Log("ZOMBIES DEALING BLOW");
            transform.forward = (target.transform.position - transform.position).normalized; // turn towards the target
            if (Physics.Raycast(damageOutputPoint.position, transform.forward, out RaycastHit hit, attackRange))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damagePerAttack);

            }

        }

    } // END DealBlow

    #endregion


} // END Class
