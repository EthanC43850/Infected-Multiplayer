using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviourPunCallbacks
{

    [HideInInspector] public PhotonView PV;

    public virtual void Awake()
    {
        PV = GetComponent<PhotonView>();

    }

    public enum Faction
    {
        Survivor,
        Zombie,
        none,

    }


} // END Class
