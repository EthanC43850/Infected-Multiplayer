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

    [Tooltip("Closest distance to follow player at")]
    public float minPatrolDistance = 7; 
    public float maxPatrolDistance = 16;
    [Tooltip("Attack enemies in this range")]
    public float enemyDetectionRadius = 7;

    public bool isHostOutOfRange;

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

        StayInRadiusOfHost();

        // Protect Player
        if (isEnemyInRange())
        {
            stateMachineScript.LookAtTarget();

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
        Collider[] hitColliders = Physics.OverlapSphere(host.position, enemyDetectionRadius); // Put layermask later
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
            else if (Vector3.Distance(host.position, _target.transform.position) > enemyDetectionRadius)
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

        // Go closer to target if out of attack range, otherwise, attack target
        if (Vector3.Distance(stateMachineScript.target.transform.position, transform.position) > stateMachineScript.attackRange)
        {
            Debug.Log("Distance between guard and target is " + Vector3.Distance(stateMachineScript.target.transform.position, transform.position));
            stateMachineScript.navMeshAgent.isStopped = false;

            stateMachineScript.navMeshAgent.SetDestination(stateMachineScript.target.transform.position);
        }
        else
        {
            stateMachineScript.navMeshAgent.isStopped = true;
            stateMachineScript.DealBlow();
        }


    } // END FindClosestTarget




    //-------------------------------------------//
    void StayInRadiusOfHost()
    {
        // If too far away or not in combat, return to host position
        if(Vector3.Distance(transform.position, host.position) > maxPatrolDistance || (stateMachineScript.target == null && Vector3.Distance(transform.position, host.position) > minPatrolDistance))
        {
            isHostOutOfRange = true; // Forget attack and move back to host for protecting
            stateMachineScript.navMeshAgent.isStopped = false;
            stateMachineScript.navMeshAgent.SetDestination(host.transform.position);
            Debug.Log("HOST OUT OF RANGE: " + Vector3.Distance(transform.position, host.position));
        }
        else if (Vector3.Distance(transform.position, host.position) < minPatrolDistance) //close enough
        {
            isHostOutOfRange = false;
            stateMachineScript.navMeshAgent.isStopped = true;

        }

    } // END isInGuardRange


    #endregion

    public void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(host.position, enemyDetectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(host.position, minPatrolDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stateMachineScript.attackRange);

    }



} // END Class
