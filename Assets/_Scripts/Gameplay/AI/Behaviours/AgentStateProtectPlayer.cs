using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public ProtectActions currentState;

/*    [Tooltip("Distance from player to run to once out of radius")]
    public float maxStoppingDistance = 7;*/
    [Tooltip("Radius to guard player from")]
    public float patrolDistance = 7;  // Distance to 
    [Tooltip("Enemy distance from host to begin attacking")]
    public float guardDistance = 7;

    public bool isReturningToPlayer;

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
        AnimateAgent();

        ReturnToPlayerRadius();

        // Protect Player
        if (isEnemyInRange())
        {
            AttackClosestTarget();
        }

    }

    private void AnimateAgent()
    {
        if(stateMachineScript.navMeshAgent.velocity.magnitude >= 0.1f)
        {
            stateMachineScript.animator.SetBool("IsMoving", true);

        }
        else
        {
            stateMachineScript.animator.SetBool("IsMoving", false);

        }


    }

    #endregion


    #region Methods

    //-------------------------------------------//
    bool isEnemyInRange() // Check for enemies in the range of spherecast
    {

        // If zombie comes close enough to player, add zombie to AI enemies list
        Collider[] hitColliders = Physics.OverlapSphere(host.position, guardDistance); // Put layermask later
        foreach (var hitCollider in hitColliders)
        {
            Targetable enemy = hitCollider.gameObject.GetComponent<Targetable>();
            
            if (enemy != null && enemy.faction != stateMachineScript.faction)
            {
                stateMachineScript.AddEnemyToList(enemy);
            }

        }

        // Remove targets that move out of distance or die
        foreach (Targetable _target in stateMachineScript.enemies.ToList())
        {
            if (_target == null || _target.isDead) // If enemy has been killed/ destroyed remove from list
            {
                stateMachineScript.enemies.Remove(_target);

            }
            else if (Vector3.Distance(host.position, _target.transform.position) > guardDistance)
            {
                stateMachineScript.enemies.Remove(_target);

            }

        }


        float closestDistance = Mathf.Infinity; // Anything closer than this will become the new target
        Targetable closestTarget = null;
        foreach (Targetable enemy in stateMachineScript.enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestTarget = enemy;
                stateMachineScript.target = closestTarget;


            }

        }




        if (stateMachineScript.enemies.Count != 0)
        {
            return true;


        }
        else
        {
            stateMachineScript.target = null; // current target has also left range
            return false;
        }



    } // END EnemyInRange




    //-------------------------------------------// ( Also located in Chase Script, should make function that allows ai to find closest object in an array to be agnostic )
    public void AttackClosestTarget() // Should AI always attack closest target?
    {

        if (Vector3.Distance(stateMachineScript.target.transform.position, transform.position) > stateMachineScript.attackRange)
        {
            //Debug.Log("Distance between guard and target is " + Vector3.Distance(stateMachineScript.target.transform.position, transform.position));
            stateMachineScript.navMeshAgent.isStopped = false;

            stateMachineScript.navMeshAgent.SetDestination(stateMachineScript.target.transform.position);
        }
        else if (!isReturningToPlayer) // 
        {
            stateMachineScript.navMeshAgent.isStopped = true;
            stateMachineScript.DealBlow();
        }


    } // END FindClosestTarget




    //-------------------------------------------//
    void ReturnToPlayerRadius()
    {
        //Debug.Log(host.position);
        if (Vector3.Distance(transform.position, host.position) < patrolDistance) //close enough
        {
            isReturningToPlayer = false;
            stateMachineScript.navMeshAgent.isStopped = true;


        }
        else
        {
            isReturningToPlayer = true; // Forget attack and move back to host for protecting
            stateMachineScript.navMeshAgent.isStopped = false;
            stateMachineScript.navMeshAgent.SetDestination(host.transform.position);



        }


    } // END isInGuardRange


    #endregion

    public void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(host.position, guardDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(host.position, patrolDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stateMachineScript.attackRange);

    }



} // END Class
