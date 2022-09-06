using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
///  Base Class for all AI Units: Zombies, Sentry Guns, Survivors
/// </summary>
public abstract class AgentStateMachine : Targetable, IDamageable
{

    #region Enums

    public enum AttackType
    {
        Melee,
        Ranged

    }


    #endregion


    #region Variables
     IAgentState currentAgentState;

    [Header("AI Unit Properties")]
    public int currentHealth;
    public int maxHealth;
    public AttackType attackType;
    public float attackRange;
    [Tooltip("Seconds between attacks")]
    public float attackCoolDown; 
    public int damagePerAttack;
    public float speed = 3.5f;
    public Item[] items;


    [Header("VFX")]
    public Animator animator;

    [Header("Sound FX")]
    [HideInInspector] public AudioClip attackAudioClip;
    [HideInInspector] public AudioClip dieAudioClip;
    [HideInInspector] public float lastBlowTime = -9999f;
    public Targetable target;
    public List<Targetable> enemies;

    [Header("AI Behaviour Scripts")]
    //public PlayerController AIController;
    public AgentStateIdle idleState;
    public AgentStateChase chaseState;
    public AgentStateAttack attackState;
    public AgentStateDie dieState;

    [Header("Additional Connections")]
    public WorldSpacePlayerUI worldSpaceUI; // Move this to its own script. this breaks SOLID Rule
    public Transform playerModelTransform;
    public Transform damageOutputPoint;
    public Transform spawnPosition;


    [HideInInspector] public NavMeshAgent navMeshAgent;


    #endregion


    #region Monobehaviours

    public virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;

        currentHealth = maxHealth;
        worldSpaceUI.SetHealthBarMax(maxHealth);

        ChangeState(idleState);
    }

    #endregion


    #region Statemachine Methods


    public void ChangeState(IAgentState newState)
    {
        if (currentAgentState != null)
        {
            currentAgentState.Exit();
        }
        currentAgentState = newState;
        currentAgentState.Enter();

    } 


    public virtual void Update()
    {
        //Debug.Log("master client is " + PhotonNetwork.IsMasterClient);
        if (!PhotonNetwork.IsMasterClient && PlayerController.debugMode == false) // Might need to add a "isPossessed" bool
        {
            return;

        }

        if (currentAgentState != null)
        {
            currentAgentState.Update();
        }
    }

    #endregion




    #region Helper Methods

 
    // BREAKS RULE OF SOLID, ONLY STATEMACHINE RELATED FUNCTIONS SHOULD BE HERE
    //-------------------------------------------//
    public virtual void DealBlow()
    {
        if (Time.time > lastBlowTime + attackCoolDown)
        {
            Debug.Log(gameObject.name + " JUST THREW A PUNCH");

            lastBlowTime = Time.time;

            if(attackType == AttackType.Melee)
            {
                animator.SetTrigger("Attack"); // Damage caused by animation event

            }
            else if(attackType == AttackType.Ranged)
            {
                items[0].Use();

                /* //test attack
                if (Physics.Raycast(damageOutputPoint.position, transform.forward, out RaycastHit hit, attackRange)) // Extended attack range a little more because nav mesh stops right at the range
                {
                    hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damagePerAttack);
                    //animator.SetTrigger("Attack");

                }*/
            }
        }

    } // END DealBlow

     
    //-------------------------------------------//
    public void LookAtTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4);



    } // END LookAt


    // BREAKS RULE OF SOLID, ONLY STATEMACHINE RELATED FUNCTIONS SHOULD BE HERE
    //-------------------------------------------//
    public void TakeDamage(int damage)
    {
        if (PlayerController.debugMode)
        {
            currentHealth -= damage;
            worldSpaceUI.UpdateHealthUI(currentHealth);
            worldSpaceUI.DisplayFloatingText(damage);
            if (currentHealth <= 0)
            {
                ChangeState(dieState);
                isDead = true;
            }
        }
        else
        {
            PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
        }

    } // END TakeDamage


    //-------------------------------------------//
    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {
        currentHealth -= damage;    // In the multiplayer tutorial, this function is placed after !PV.IsMine. Keep this in mind if problems arise in the future.
        worldSpaceUI.UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
        {
            ChangeState(dieState);
            isDead = true;

        }

    } // END RPC_TakeDamage


    //-------------------------------------------//
    void ResetZombie()
    {
        currentHealth = maxHealth;
        worldSpaceUI.UpdateHealthUI(currentHealth);
        transform.position = spawnPosition.position;
    }



    //-------------------------------------------//
    public void AddEnemyToList(Targetable enemy)
    {
        
        if (faction != enemy.faction) // Make sure AI enemies are not on same team
        {
            foreach(Targetable _enemy in enemies) // Go through current list of targets to make sure target is not added twice
            {
                if(enemy == _enemy)
                {
                    return;
                }

            }

            if (enemy.isDead)
            {
                return;

            }

            enemies.Add(enemy);

        }

    } // END AddEnemyToList




    #endregion


} // END Abstract Class
