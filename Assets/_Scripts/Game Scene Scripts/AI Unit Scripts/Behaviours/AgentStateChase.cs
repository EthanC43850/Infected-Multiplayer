using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateChase : MonoBehaviour, IAgentState
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
        Debug.Log("Entered Chase");

    }

    public void Exit()
    {
        
    }

    void IAgentState.Update()
    {

        ChaseClosestTarget();


        if (IsTargetInRange())
        {
            stateMachineScript.ChangeState(stateMachineScript.attackState);
        }




    } // END Update

    #endregion

    #region Helper Methods

    //-------------------------------------------//
    public void ChaseClosestTarget() // Should AI always attack closest target?
    {
        
        //Debug.Log("looking for closest target");
        float closestDistance = Mathf.Infinity; // Anything closer than this will become the new target
        Targetable closestTarget = null;

        foreach (Targetable enemy in stateMachineScript.enemies)
        {
            Debug.Log("SURVIVOR " + enemy.name + " is DISTANCE: " + Vector3.Distance(transform.position, enemy.transform.position));
            if(enemy == null) // If enemy has been destroyed remove from list
            {
                stateMachineScript.enemies.Remove(enemy);
            }


            if (Vector3.Distance(transform.position, enemy.transform.position) < closestDistance)
            {
                closestTarget = enemy;


            }

        }

        if (closestTarget == null)
        {
            Debug.Log("No targets found!");
        }
        else
        {
            stateMachineScript.target = closestTarget;
            stateMachineScript.navMeshAgent.SetDestination(closestTarget.transform.position);
            Debug.Log("Closest target is " + closestTarget.gameObject.name);

        }

    } // END FindClosestTarget



    //-------------------------------------------//
    public bool IsTargetInRange()
    {
        return (transform.position - stateMachineScript.target.transform.position).magnitude <= stateMachineScript.attackRange;

    } // END IsTargetInRange



    #endregion




} // END Class
