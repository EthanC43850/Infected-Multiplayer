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
    [serializefield] float guardDistance;



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
    }

    public void Exit()
    {


    }

    void IAgentState.Update()
    {
        Debug.Log(host.transform.position);

        if (isInGuardRange())
        {
            stateMachineScript.navMeshAgent.isStopped = true;
            
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
        Collider[] hitColliders = Physics.OverlapSphere(host.position, guardDistance); // Put layermask later
        foreach(var hitCollider in hitColliders)
        {
            Zombie_Unit zombie = hitCollider.gameObject.GetComponent<Zombie_Unit>();
            if (zombie != null)
            {
                stateMachineScript.target = zombie;
                stateMachineScript.ChangeState(stateMachineScript.chaseState);
                break;
            }



        }


    }




    bool isInGuardRange()
    {

        if (!stateMachineScript.navMeshAgent.pathPending)
        {
            Debug.Log("Distance from host is " + stateMachineScript.navMeshAgent.remainingDistance);

            if (stateMachineScript.navMeshAgent.remainingDistance <= stateMachineScript.navMeshAgent.stoppingDistance + 7)
            {
                if (!stateMachineScript.navMeshAgent.hasPath || stateMachineScript.navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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
        Gizmos.DrawSphere(host.position, guardDistance);

    }



} // END Class
