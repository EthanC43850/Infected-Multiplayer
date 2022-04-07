using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class Zombie_Unit : AI_Unit
{
    #region Variables




    private NavMeshAgent navMeshAgent;

    #endregion


    #region Monobehaviours
    public override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();

    }


    public override void Update()
    {
        base.Update();

    }


    #endregion


    #region Methods

    public override void Seek()
    {
        base.Seek();
        navMeshAgent.SetDestination(target.transform.position);

    } // END Seek

    public override void StartAttack()
    {
        base.StartAttack();
        state = States.Attacking;

    } // END StartAttack


    #endregion


}
