using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Base Class for all AI Units: Zombies, Sentry Guns, Survivors
/// </summary>
public class AI_Unit : MonoBehaviour
{

    public enum States
    {
        Idle, // When the unit is first being spawned in
        Following, // Follows the creator
        Defending, // Guards immediate vacinity
        Seeking, // Searches for closest target in range
        Attacking, // Attack cycle with animation, not moving
        Fleeing // AI is either weak or has encountered a repellant

    }

    public enum AttackType
    {
        Melee,
        Ranged

    }

    [Header("AI Unit Properties")]
    public AttackType attackType;
    public int health;
    public float attackRange;
    public float attackRatio; // Time between attacks
    public float damagePerAttack;
    public float speed; // Movement speed



    [Header("Sound FX")]
    [HideInInspector] public AudioClip attackAudioClip;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
