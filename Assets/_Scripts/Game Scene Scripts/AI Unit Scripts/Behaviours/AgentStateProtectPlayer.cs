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
    public ProtectActions currentState;
    [Tooltip("Distance to follow player from")]
    public float patrolDistance = 7;  // Distance to 
    public float guardDistance = 7;

    bool isReturningToPlayer;

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



        ReturnToPlayerRadius();

        // Protect Player
        if (isEnemyInRange())
        {
            AttackClosestTarget();

        }


    }

    #endregion


    #region Methods

    bool isEnemyInRange()
    {


        // If zombie comes close enough to player, add zombie to AI enemies list
        Collider[] hitColliders = Physics.OverlapSphere(host.position, guardDistance); // Put layermask later
        foreach (var hitCollider in hitColliders)
        {
            Zombie_Unit zombie = hitCollider.gameObject.GetComponent<Zombie_Unit>();
            if (zombie != null)
            {
                stateMachineScript.AddEnemyToList(zombie);
            }

        }

        // Remove targets that move out of distance or die
        foreach (Targetable _target in stateMachineScript.enemies)
        {
            if (_target == null)
            {
                if (Vector3.Distance(host.position, _target.transform.position) > guardDistance)
                {
                    stateMachineScript.enemies.Remove(_target);

                }
            }
        }


        float closestDistance = Mathf.Infinity; // Anything closer than this will become the new target
        Targetable closestTarget = null;
        foreach (Targetable enemy in stateMachineScript.enemies)
        {
            if (enemy == null) // If enemy has been killed/ destroyed remove from list
            {
                stateMachineScript.enemies.Remove(enemy);
            }

            if ((transform.position - enemy.transform.position).magnitude < closestDistance)
            {
                closestTarget = enemy;

            }

        }

        if (closestTarget != null) // If null, that means no enemies in range
        {
            stateMachineScript.target = closestTarget;

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

        if (Vector3.Distance(stateMachineScript.target.transform.position, host.position) > stateMachineScript.attackRange && isReturningToPlayer)
        {
            stateMachineScript.navMeshAgent.isStopped = false;

            stateMachineScript.navMeshAgent.SetDestination(stateMachineScript.target.transform.position);
        }
        else // Close enough for attack
        {
            stateMachineScript.navMeshAgent.isStopped = true;
            stateMachineScript.DealBlow();
        }


    } // END FindClosestTarget




    //-------------------------------------------//
    void ReturnToPlayerRadius()
    {

        if (Vector3.Distance(transform.position, host.position) < patrolDistance) //close enough
        {
            isReturningToPlayer = false;
            stateMachineScript.navMeshAgent.isStopped = true;


        }
        else
        {
            isReturningToPlayer = true; // Forget attack and move to player
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


    }



} // END Class
