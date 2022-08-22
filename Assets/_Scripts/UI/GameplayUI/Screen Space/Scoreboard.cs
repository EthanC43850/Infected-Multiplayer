using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;


/// <summary>
/// Manages Scoreboard Items for when players leave and join
/// </summary>
public class Scoreboard : MonoBehaviourPunCallbacks
{

    [SerializeField] Transform container;
    [SerializeField] GameObject scoreBoardItemPrefab;

    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);

        }
    }

    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreBoardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Init(player);
        scoreboardItems[player] = item;
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);

    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }





} // END Class
