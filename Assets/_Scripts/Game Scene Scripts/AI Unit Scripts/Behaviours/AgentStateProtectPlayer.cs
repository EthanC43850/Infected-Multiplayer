using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateProtectPlayer : MonoBehaviour, IAgentState
{

    public enum ProtectActions
    {
        Guarding,
        Retreating,
        Fighting


    }

    // NOTE: Add functionality where, if player enters combat he'll keep fighting until the player decides to run away
    #region Variables

    [Header("Player Protect Properties")]
    public float patrolDistance = 7;  // Distance to 
    public float guardDistance = 7;



    [Header("Additional Components")]
    public Transform host;

    // Add navmesh agents to each script instaed of referencing statemachine
    public AgentStateMachine stateMachineScript;

    #endregion


    #region Monobehaviours

    public void Awake()
    {
        stateMachineScript = GetComponent<AgentStateMachine>();
    }

    public void Enter()
    {
        Debug.Log("ENTERED PROTECT MODE");
        stateMachineScript.navMeshAgent.SetDestination(host.transform.position);
        stateMachineScript.navMeshAgent.isStopped = false;

    }

    public void Exit()
    {


    }

    void IAgentState.Update()
    {


        // Protect Player
        ProtectPlayer();


        // Follow Player
        if (isInGuardRange())
        {
            stateMachineScript.navMeshAgent.isStopped = true;
            // create coroutine to change patrol distance every few seconds for more realism?
        }
        else
        {

            stateMachineScript.navMeshAgent.SetDestination(host.transform.position);
            stateMachineScript.navMeshAgent.isStopped = false;

        }






    }

    #endregion


    #region Methods

    void ProtectPlayer()
    {

        // If zombie comes close enough to player, AI will target zombie and get in range to kill
        Collider[] hitColliders = Physics.OverlapSphere(host.position, guardDistance); // Put layermask later
        foreach(var hitCollider in hitColliders)
        {
            Zombie_Unit zombie = hitCollider.gameObject.GetComponent<Zombie_Unit>();
            if (zombie != null && stateMachineScript.target != null) // Make sure only 1 zombie is targetted at a time
            {
                stateMachineScript.target = zombie;
                stateMachineScript.ChangeState(stateMachineScript.chaseState);
                break;
            }

        }

        if (!isTargetInRangeOfPlayer())
        {
            stateMachineScript.target = null;

        }



    }




    bool isInGuardRange()
    {

        if (Vector3.Distance(transform.position, host.position) < patrolDistance)
        {
            return true;
        }
        else
        {
            return false;

        }


    } // END isInGuardRange

    bool isTargetInRangeOfPlayer()
    {

        if(Vector3.Distance(stateMachineScript.target.transform.position, host.position) > guardDistance)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    #endregion

    public void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(host.position, guardDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(host.position, patrolDistance);


    }



} // END Class
