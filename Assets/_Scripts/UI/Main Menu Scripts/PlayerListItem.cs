using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviourPunCallbacks
{

    Player player;
    [SerializeField] Text text;

    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;


    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }

    }


    //Figure out why this function won't call on the local client (Bug appears when leaving and rejoining a room)
    public override void OnLeftRoom()
    {
        Debug.Log("JUST LEFT ROOM DELETEEE MEEEE");
        Destroy(gameObject);
    }

}
