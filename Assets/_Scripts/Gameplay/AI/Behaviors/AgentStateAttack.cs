using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateAttack : MonoBehaviour, IAgentState
{

    #region Variables

    AgentStateMachine stateMachineScript;
    public GameObject debugCube;

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


        /*Instantiate(debugCube, transform.position, Quaternion.identity);
        Instantiate(debugCube, stateMachineScript.target.transform.position, Quaternion.identity);*/
    }

    public void Exit()
    {
            stateMachineScript.navMeshAgent.isStopped = false;
    }

    void IAgentState.Update()
    {

        //Debug.Log("Vector3.distance is " + Vector3.Distance(transform.position, stateMachineScript.target.transform.position) + " and navmeshdistance is " + stateMachineScript.navMeshAgent.remainingDistance);
        //Debug.Log("Distance from zombie to target is " + Vector3.Distance(stateMachineScript.target.transform.position, transform.position)); // THIS PRINTS WEIRD NUMBERS, may become a problem in future

        if (!IsTargetInRangeOrDead()) // Back to chasing if target is out of range or dies
        {
            //Debug.Log("TARGET GOT OUT OF RANGE OR DIED");
            stateMachineScript.ChangeState(stateMachineScript.chaseState);
        }

        stateMachineScript.LookAtTarget();

        stateMachineScript.DealBlow();


    }

    #endregion

    #region Methods


    //-------------------------------------------//
    public bool IsTargetInRangeOrDead()
    {
        if(stateMachineScript.target == null || stateMachineScript.target.isDead)
        {

            return false;
        }

        return Vector3.Distance(stateMachineScript.target.transform.position, transform.position) <= stateMachineScript.attackRange;

    } // END IsTargetInRange


    #endregion

} // END Class
