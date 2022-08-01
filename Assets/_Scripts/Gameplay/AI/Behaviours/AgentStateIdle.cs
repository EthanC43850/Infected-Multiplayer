using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateIdle : MonoBehaviour, IAgentState
{
    #region Variables
    AgentStateMachine stateMachineScript;



    #endregion


    #region Monobehaviours

    public void Awake()
    {
        stateMachineScript = GetComponent<AgentStateMachine>();
    }


    public void Enter()
    {

        stateMachineScript.navMeshAgent.isStopped = true;
        stateMachineScript.animator.SetBool("IsMoving", false);

    }

    public void Exit()
    {

    }


    void IAgentState.Update()
    {
        if (stateMachineScript.enemies.Count != 0)
        {
            stateMachineScript.ChangeState(stateMachineScript.chaseState);
        }
        //Debug.Log(stateMachineScript.enemies.Count);

        if (stateMachineScript.enemies.Count == 0 && PlayerController.debugMode)
        {

            foreach (Targetable target in FindObjectsOfType<Targetable>())
            {
                //Debug.Log(target.name);
                stateMachineScript.AddEnemyToList(target);

            }

        }

    }
    #endregion

   




} // END Class
