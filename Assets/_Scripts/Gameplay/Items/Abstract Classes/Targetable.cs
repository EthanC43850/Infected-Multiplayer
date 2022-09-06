using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all objects that can be damaged (Units, players, structures, buildings)
// Can assign different teams/ factions, as well as having units target a specific type of faction
public class Targetable : MonoBehaviourPunCallbacks
{
    public Faction faction;
    public bool isDead;

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
