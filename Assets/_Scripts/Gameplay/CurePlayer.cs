using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurePlayer : MonoBehaviour
{
    
    public void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player == null) { return; }
        
        if(player.PV.IsMine)
        {
            Debug.Log("ENTERED TRIGGER:  " + player.gameObject.name);
            player.playerManager.RespawnAsSurvivor();    
        }

    }


} // END Class
