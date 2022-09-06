using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New_AI_Unit", menuName = "AI Data")]
public class AiUnitInfo : ScriptableObject
{
    public string unitName;

    [Header("AI Unit Properties")]
    public int maxHealth;
    public float speed;     // Movement speed
    public AgentStateMachine.AttackType attackType;
    public float attackRange;
    public float attackRatio;   // Time between attacks
    public float damagePerAttack;

    [Header("Sound FX")]
    [HideInInspector] public AudioClip attackAudioClip;
    [HideInInspector] public AudioClip dieAudioClip;

} // END Class
