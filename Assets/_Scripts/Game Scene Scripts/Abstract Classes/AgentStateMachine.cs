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
    public float attackRatio; // Time between attacks
    public int damagePerAttack;
    public float speed; // Movement speed

    public WorldSpacePlayerUI worldSpaceUI; // Move this to its own script. this breaks SOLID Rule


    [Header("Sound FX")]
    [HideInInspector] public AudioClip attackAudioClip;
    [HideInInspector] public AudioClip dieAudioClip;
    [HideInInspector] public float lastBlowTime = -500;
    public Targetable target;
    public List<Targetable> enemies;

    public Transform spawnPosition;

    [HideInInspector] public NavMeshAgent navMeshAgent;

    [Header("AI Behaviour Scripts")]
    public AgentStateIdle idleState;
    public AgentStateChase chaseState;
    public AgentStateAttack attackState;


    #endregion


    #region Monobehaviours



    public virtual void Start()
    {
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
        lastBlowTime = Time.time;

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
                Die();
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
            Die();
        }

    } // END RPC_TakeDamage


    //-------------------------------------------//
    void Die()
    {
        //gameObject.SetActive(false);
        Debug.Log("ZOMBIE SHOULD'VE DIED");
        health = maxHealth;
        worldSpaceUI.UpdateHealthUI(health);
        transform.position = spawnPosition.position;


    } // END Die


    //-------------------------------------------//
    public void AddEnemyToList(Targetable enemy)
    {
        
        if (faction != enemy.faction)
        {
            enemies.Add(enemy);

        }

    } // END AddEnemyToList







    #endregion





} // END Abstract Class
