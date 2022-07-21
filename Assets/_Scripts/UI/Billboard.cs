using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


/// <summary>
/// This Script ensures that the player user name is always facing the camera
/// </summary>
public class Billboard : MonoBehaviour
{

    CinemachineBrain mainCam;

    private void Awake()
    {
        mainCam = FindObjectOfType<CinemachineBrain>();

    }
    void Update()
    {
        if (mainCam != null)
        {
            transform.LookAt(mainCam.transform);
            transform.Rotate(Vector3.up * 180);
        }


    }
}
