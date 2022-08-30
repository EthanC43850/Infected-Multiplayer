using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class Zombie_Unit : AgentStateMachine
{
    #region Variables




    #endregion


    #region Monobehaviours

    public override void Start()
    {
        base.Start();
        if (faction == Faction.Zombie)
        {
            spawnPosition = FindObjectOfType<ZombieSpawnPoint>().transform;
        }

    }



    #endregion


    #region Methods


    #endregion


} // END Class
