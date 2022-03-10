using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    SpawnPoint[] respawnPoints;

    private void Awake()
    {
        Instance = this;
        respawnPoints = GetComponentsInChildren<SpawnPoint>();
    }


    public Transform GetSpawnpoint()
    {
        return respawnPoints[Random.Range(0, respawnPoints.Length)].transform;
    }
    
}
