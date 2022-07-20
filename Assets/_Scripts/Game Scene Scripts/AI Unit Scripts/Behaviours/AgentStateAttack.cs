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
        stateMachineScript.animator.SetBool("IsMoving", false);

    }

    public void Exit()
    {
            stateMachineScript.navMeshAgent.isStopped = false;
    }

    void IAgentState.Update()
    {

        //Debug.Log("Vector3.distance is " + Vector3.Distance(transform.position, stateMachineScript.target.transform.position) + " and navmeshdistance is " + stateMachineScript.navMeshAgent.remainingDistance);
        Debug.Log("The name of the current transform is " + transform.name);

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

        return Vector3.Distance(transform.position, stateMachineScript.target.transform.position) <= stateMachineScript.attackRange;

    } // END IsTargetInRange


    #endregion

} // END Class
