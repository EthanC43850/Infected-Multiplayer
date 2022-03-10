using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] Menu[] menuBank;


    private void Awake()
    {
        Instance = this;

    } // END Awake


    public void OpenMenu(string menuName)
    {
        foreach(Menu _menu in menuBank)
        {
            if(_menu.menuName == menuName)
            {
                OpenMenu(_menu);
            }
            else if (_menu.isOpen)
            {
                CloseMenu(_menu);
            }
        }

    }


    public void OpenMenu(Menu _menu)
    {
        for(int i = 0; i < menuBank.Length; i++)
        {
            if (menuBank[i].isOpen)
            {
                CloseMenu(menuBank[i]);
            }
        }
        _menu.Open();

    }


    public void CloseMenu(Menu _menu)
    {
        _menu.Close();

    }

    public void ChangeUsername(string username)
    {
        PhotonNetwork.NickName = username;

    }

}
