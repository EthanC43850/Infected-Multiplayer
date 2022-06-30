using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateAttack : MonoBehaviour, IAgentState
{

    #region Variables
    AgentStateMachine stateMachineScript;

    #endregion


    #region Monobehaviours

    public void Awake()
    {
        stateMachineScript = GetComponent<AgentStateMachine>();
    }

    public void Enter()
    {
        Debug.Log("Entered Attack state");

        stateMachineScript.navMeshAgent.isStopped = true;

    }

    public void Exit()
    {
            stateMachineScript.navMeshAgent.isStopped = false;
    }

    void IAgentState.Update()
    {
        if (!IsTargetInRangeOrDead()) // Back to chasing if target is out of range or dies
        {
            stateMachineScript.ChangeState(stateMachineScript.chaseState);
        }

        stateMachineScript.DealBlow();


    } 

    #endregion

    #region Methods

    //-------------------------------------------//
    public bool IsTargetInRangeOrDead()
    {
        if(stateMachineScript.target == null)
        {
            return false;
        }

        return (transform.position - stateMachineScript.target.transform.position).magnitude <= stateMachineScript.attackRange;

    } // END IsTargetInRange


    #endregion

} // END Class
