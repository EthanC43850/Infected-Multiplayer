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
    public int health;
    public int maxHealth = 50;
    public AttackType attackType;
    public float attackRange;
    [Tooltip("Seconds between attacks")]
    public float attackCoolDown; // Seconds between attacks
    public int damagePerAttack;
    public float speed = 3.5f; // Movement speed

    public WorldSpacePlayerUI worldSpaceUI; // Move this to its own script. this breaks SOLID Rule

    [Header("VFX")]
    public Animator animator;
    //public float maxSpeedForWalkAnimation;


    [Header("Sound FX")]
    [HideInInspector] public AudioClip attackAudioClip;
    [HideInInspector] public AudioClip dieAudioClip;
    [HideInInspector] public float lastBlowTime = -9999f;
    public Targetable target;
    public List<Targetable> enemies;

    public Transform spawnPosition;

    [HideInInspector] public NavMeshAgent navMeshAgent;

    [Header("AI Behaviour Scripts")]
    public AgentStateIdle idleState;
    public AgentStateChase chaseState;
    public AgentStateAttack attackState;
    public AgentStateDie dieState;
    public Transform damageOutputPoint;


    #endregion


    #region Monobehaviours



    public virtual void Start()
    {
        if(faction == Faction.Zombie)
        {
            spawnPosition = FindObjectOfType<ZombieSpawnPoint>().transform;
        }


        //Subscribe function
        PlayerManager.AddEnemyToAILists += AddEnemyToList;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;

        worldSpaceUI.SetHealthBarMax(maxHealth);
        ChangeState(idleState);
    }


    

    public void Update()
    {
        if (!PhotonNetwork.IsMasterClient && PlayerController.debugMode == false)
        {
            return;
        }

        if (currentAgentState != null)
        {

            currentAgentState.Update();
        }

        
    }

    #endregion



    #region Methods

    //-------------------------------------------//
    public void ChangeState(IAgentState newState)
    {
        if(currentAgentState != null)
        {
            currentAgentState.Exit();
        }
        currentAgentState = newState;
        currentAgentState.Enter();

    } // END ChangeState





    //-------------------------------------------//
    public virtual void DealBlow()
    {
        Debug.Log("PUNCH PUNCH PUNCH");
        if (Time.time > lastBlowTime + attackCoolDown)
        {
            lastBlowTime = Time.time;

            transform.forward = (target.transform.position - transform.position).normalized; // turn towards the target
            if (Physics.Raycast(damageOutputPoint.position, transform.forward, out RaycastHit hit, attackRange+1)) // Extended attack range a little more because nav mesh stops right at the range
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damagePerAttack);
                animator.SetTrigger("Attack");

            }

        }


    } // END DealBlow



    // BREAKS RULE OF SOLID, ONLY STATEMACHINE RELATED FUNCTIONS SHOULD BE HERE
    //-------------------------------------------//
    public void TakeDamage(int damage)
    {
        if (PlayerController.debugMode)
        {
            health -= damage;
            worldSpaceUI.UpdateHealthUI(health);
            worldSpaceUI.DisplayFloatingText(damage);
            if (health <= 0)
            {
                ChangeState(dieState);
            }
        }
        else
        {
            PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        }

    } // END TakeDamage


    //-------------------------------------------//
    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {

        if (!PV.IsMine) { return; }         // Pun RPC runs on everyones computer, but the !pv.ismine makes sure the function only runs on the victims computer

        health -= damage;    // In the multiplayer tutorial, this function is placed after !PV.IsMine. Keep this in mind if problems arise in the future.
        worldSpaceUI.UpdateHealthUI(health);

        if (health <= 0)
        {
            ChangeState(dieState);

        }

    } // END RPC_TakeDamage


    //-------------------------------------------//
    void ResetZombie()
    {
        health = maxHealth;
        worldSpaceUI.UpdateHealthUI(health);
        transform.position = spawnPosition.position;
    }



    //-------------------------------------------//
    public void AddEnemyToList(Targetable enemy)
    {
        
        if (faction != enemy.faction) // Make sure AI enemies are not on same team
        {
            foreach(Targetable _enemy in enemies) // Make sure not to add same enemies twice
            {
                if(enemy == _enemy)
                {
                    return;
                }
            }

            enemies.Add(enemy);

        }

    } // END AddEnemyToList




    #endregion





} // END Abstract Class
