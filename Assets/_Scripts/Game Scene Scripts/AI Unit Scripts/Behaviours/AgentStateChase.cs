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
        if(stateMachineScript.target == null)   
        {
            FindClosestTarget();

        }

        if (IsTargetInRange())
        {
            stateMachineScript.ChangeState(stateMachineScript.attackState);
        }




    } // END Update

    #endregion

    #region Helper Methods

    //-------------------------------------------//
    public void FindClosestTarget() // Should AI always attack closest target?
    {
        
        //Debug.Log("looking for closest target");
        float closestDistance = Mathf.Infinity; // Anything closer than this will become the new target
        Targetable closestTarget = null;

        foreach (Targetable enemy in stateMachineScript.enemies)
        {
            //Debug.Log(enemy.name);
            if(enemy == null) // If enemy has been destroyed remove from list
            {
                stateMachineScript.enemies.Remove(enemy);
            }


            if ((transform.position - enemy.transform.position).magnitude < closestDistance)
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
        }

    } // END FindClosestTarget



    //-------------------------------------------//
    public bool IsTargetInRange()
    {
        return (transform.position - stateMachineScript.target.transform.position).magnitude <= stateMachineScript.attackRange;

    } // END IsTargetInRange



    #endregion




} // END Class
