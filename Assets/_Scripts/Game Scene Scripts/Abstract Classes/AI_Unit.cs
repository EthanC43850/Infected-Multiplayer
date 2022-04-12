using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Base Class for all AI Units: Zombies, Sentry Guns, Survivors
/// </summary>
public abstract class AI_Unit : Targetable, IDamageable
{

    #region Enums

    public States state = States.Idle;

    public enum States
    {
        Idle, // When the unit is first being spawned in
        Following, // Follows the creator
        Defending, // Guards immediate vacinity
        Seeking, // Searches for closest target in range
        Attacking, // Attack cycle with animation, not moving
        Fleeing, // AI is either weak or has encountered a repellant. Survivor AI is backing away from another zombie. (implement functionality for reloading)
        Dead

    }

    public enum AttackType
    {
        Melee,
        Ranged

    }


    #endregion


    #region Variables

    [Header("AI Unit Properties")]
    public int health;
    public AttackType attackType;
    public float attackRange;
    public float attackRatio; // Time between attacks
    public int damagePerAttack;
    public float speed; // Movement speed

    public WorldSpacePlayerUI worldSpaceUI;

    [Header("Sound FX")]
    [HideInInspector] public AudioClip attackAudioClip;
    [HideInInspector] public AudioClip dieAudioClip;

    [HideInInspector] public float lastBlowTime = -1000f;
    public Targetable target;
    public List<Targetable> enemies;

    public Transform spawnPosition;

    #endregion


    #region Monobehaviours

    public virtual void Start()
    {
        worldSpaceUI.SetHealthBarMax(health);
    }


    public virtual void Update()
    {
        /*if (!PhotonNetwork.IsMasterClient && !PlayerController.debugMode)
        {

            return;
        }*/

        switch (state)
        {
            case States.Idle:
                bool targetFound = FindClosestTarget();
                if (targetFound)
                {
                    Seek();
                }

                break;
            case States.Seeking:
                Seek();     // Could delete this and call seek coroutine in the seek function to allow delay in destination change
                if (IsTargetInRange())
                {
                    StartAttack();
                }

                break;
            case States.Attacking:
                if (IsTargetInRange()) {

                    if(Time.time >= lastBlowTime + attackRatio)
                    {
                        DealBlow();

                    }
                
                }
                else
                {
                    Seek();
                }

                break;

        }


    }

    #endregion



    #region Methods

    public virtual void Seek()
    {
        state = States.Seeking;
        

    } // END Seek


    public virtual void StartAttack()
    {
        state = States.Attacking;

    } // END StartAttack

    public virtual void DealBlow()
    {
        Debug.Log("PUNCH PUNCH PUNCH");
        lastBlowTime = Time.time;

    } // END DealBlow


    public virtual void Stop()
    {
        state = States.Idle;

    } // END Stop


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

    void Die()
    {
        //gameObject.SetActive(false);
        Debug.Log("ZOMBIE SHOULD'VE DIED");
        transform.position = spawnPosition.position;


    } // END Die


    


    #endregion


    #region Helper Methods

    public bool FindClosestTarget()
    {
        Debug.Log("looking for closest target");
        float closestDistance = Mathf.Infinity; // Anything closer than this will become the new target
        Targetable closestTarget = null;

        foreach (Targetable enemy in enemies)
        {
            if ((transform.position - enemy.transform.position).magnitude < closestDistance)
            {
                closestTarget = enemy;

            }

        }

        if (closestTarget == null)
        {
            Debug.Log("No targets found!");
            return false;
        }
        else
        {
            target = closestTarget;
            return true;
        }

    } // END FindClosestTarget


    public virtual void SetTarget(Targetable _target)
    {
        target = _target;

    } // END SetTarget


    public bool IsTargetInRange()
    {
        return (transform.position - target.transform.position).magnitude <= attackRange;

    } // END IsTargetInRange

    #endregion


} // END Abstract Class
