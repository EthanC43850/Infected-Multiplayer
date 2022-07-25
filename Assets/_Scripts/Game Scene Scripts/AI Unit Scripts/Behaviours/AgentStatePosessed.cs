using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class AgentStatePosessed : MonoBehaviour, IAgentState 
{

    AgentStateMachine stateMachineScript;
    PlayerController AIController;

    public Animator AIAnimator;
    public AnimatorController zombiePossessedAnimator;
    public AnimatorController zombieAIAnimator;

    private void Awake()
    {
        stateMachineScript = GetComponent<AgentStateMachine>();
        AIController = GetComponent<PlayerController>();

    }

    public void Enter()
    {
        stateMachineScript.navMeshAgent.enabled = false;
        ((ZombieController)AIController).isPossessed = true;    // Works only for zombie atm


    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }


    void IAgentState.Update()
    {
        throw new System.NotImplementedException();
    }


}
