using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateIdle : MonoBehaviour, IAgentState
{
    #region Variables
    AgentStateMachine stateMachineScript;

    #endregion


    #region Monobehaviours

    public void Start()
    {
        stateMachineScript = GetComponent<AgentStateMachine>();
    }


    public void Enter()
    {
        Debug.Log("Entered Idle");
        stateMachineScript.navMeshAgent.isStopped = true;
    }

    public void Exit()
    {
        stateMachineScript.navMeshAgent.isStopped = false;

    }


    void IAgentState.Update()
    {
        if (stateMachineScript.enemies.Count != 0)
        {
            stateMachineScript.ChangeState(stateMachineScript.chaseState);
        }
    }
    #endregion

   




} // END Class
