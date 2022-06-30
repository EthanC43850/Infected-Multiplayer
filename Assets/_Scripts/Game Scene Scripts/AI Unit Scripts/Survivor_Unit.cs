using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Survivor_Unit : AgentStateMachine
{
    #region Variables

    [Header("Additional Survivor Behaviours")]
    public AgentStateProtectPlayer protectState;



    #endregion


    #region Monobehaviours

    public override void Start()
    {
        base.Start();

        ChangeState(protectState);
    }



    #endregion


    #region Methods






    #endregion
}
