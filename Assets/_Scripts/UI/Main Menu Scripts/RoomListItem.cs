using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] Text roomName;

    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        roomName.text = _info.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }

}
