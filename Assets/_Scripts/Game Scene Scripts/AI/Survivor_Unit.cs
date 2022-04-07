using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Survivor_Unit : AI_Unit
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






    #endregion
}
