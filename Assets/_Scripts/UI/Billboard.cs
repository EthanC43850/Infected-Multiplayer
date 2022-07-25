using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


/// <summary>
/// This Script ensures that the player user name is always facing the camera
/// </summary>
public class Billboard : MonoBehaviour
{

    Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

    }
    void Update()
    {
        if (mainCam != null)
        {
            transform.LookAt(mainCam.transform);
            //transform.Rotate(Vector3.up * 180);
        }


    }
}
