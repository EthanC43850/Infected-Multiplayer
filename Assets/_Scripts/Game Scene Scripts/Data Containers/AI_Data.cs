using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New_AI_Unit", menuName = "AI Data")]
public class AI_Data : ScriptableObject
{
    public string unitName;

    [Header("AI Unit Properties")]
    public AI_Unit.AttackType attackType;
    public float attackRange;
    public float attackRatio;   // Time between attacks
    public float damagePerAttack;
    public int health;
    public float speed;     // Movement speed

    [Header("Sound FX")]
    [HideInInspector] public AudioClip attackAudioClip;
    [HideInInspector] public AudioClip dieAudioClip;



} // END Class
