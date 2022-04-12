using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class Zombie_Unit : AI_Unit
{
    #region Variables


    [Header("Connections")]
    public Transform damageOutputPoint;
    private NavMeshAgent navMeshAgent;

    #endregion


    #region Monobehaviours
    public override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.speed = speed;

    }


    public override void Update()
    {
        base.Update();
        Debug.DrawRay(damageOutputPoint.position, transform.forward, Color.green, attackRange);
    }


    #endregion


    #region Methods

    public override void Seek()
    {
        if(target == null)
        {
            enemies.Remove(target);
            return;
        }

        base.Seek();
        navMeshAgent.SetDestination(target.transform.position);

    } // END Seek

    public override void StartAttack()
    {
        base.StartAttack();
        navMeshAgent.isStopped = true;
        // animator.SetBool("IsMoving", false);

    } // END StartAttack


    public override void DealBlow()
    {
        base.DealBlow();

        transform.forward = (target.transform.position - transform.position).normalized; // turn towards the target
        if(Physics.Raycast(damageOutputPoint.position, transform.forward, out RaycastHit hit, attackRange))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damagePerAttack);

        }


    } // END DealBlow

    #endregion


} // END Class
