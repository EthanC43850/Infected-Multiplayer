using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        //Debug.Log("Entered Chase");
        stateMachineScript.navMeshAgent.isStopped = false;
        stateMachineScript.animator.SetBool("IsMoving", true);


    }

    public void Exit()
    {
        
    }

    void IAgentState.Update()
    {

        ChaseClosestTarget();

        stateMachineScript.LookAtTarget();

        if (IsTargetInRange())
        {
            stateMachineScript.ChangeState(stateMachineScript.attackState);
        }




    } // END Update

    #endregion

    #region Helper Methods



    //-------------------------------------------//
    public void ChaseClosestTarget() 
    {
        
        float closestDistance = Mathf.Infinity; // Anything closer than this will become the new target
        Targetable closestTarget = null;

        foreach (Targetable enemy in stateMachineScript.enemies.ToList())
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (enemy == null || enemy.isDead) // If enemy has been destroyed remove from list
            {
                stateMachineScript.enemies.Remove(enemy);
            }


            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestTarget = enemy;
                stateMachineScript.target = closestTarget;
                stateMachineScript.navMeshAgent.SetDestination(closestTarget.transform.position);
                //Debug.Log("Closest target is " + closestTarget.gameObject.name);

            }

        }

        if (closestTarget == null)
        {
            Debug.Log("No targets found!");
        }


    } // END FindClosestTarget



    //-------------------------------------------//
    public bool IsTargetInRange()
    {
        
        return Vector3.Distance(transform.position, stateMachineScript.target.transform.position) <= stateMachineScript.attackRange;

    } // END IsTargetInRange



    #endregion




} // END Class
