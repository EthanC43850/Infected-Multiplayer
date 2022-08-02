using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateDie : MonoBehaviour, IAgentState
{

    AgentStateMachine stateMachineScript;
    public Collider hitBox;

    public void Awake()
    {
        stateMachineScript = GetComponent<AgentStateMachine>();


    }

    void IAgentState.Enter()
    {
        hitBox.enabled = false;
        stateMachineScript.navMeshAgent.enabled = false;
        stateMachineScript.animator.SetTrigger("IsDead");
        stateMachineScript.worldSpaceUI.gameObject.SetActive(false);
        stateMachineScript.isDead = true;

    }

    void IAgentState.Exit()
    {

    }



    void IAgentState.Update()
    {

    }
} // END Class
