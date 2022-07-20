using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateDie : MonoBehaviour, IAgentState
{

    AgentStateMachine stateMachineScript;

    public void Awake()
    {
        stateMachineScript = GetComponent<AgentStateMachine>();


    }

    void IAgentState.Enter()
    {

        stateMachineScript.navMeshAgent.enabled = false;
        stateMachineScript.animator.SetTrigger("IsDead");


    }

    void IAgentState.Exit()
    {

    }



    void IAgentState.Update()
    {

    }
} // END Class
