using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnPoint : MonoBehaviour
{


    [SerializeField] bool isSpawning;
    [SerializeField] float zombieSpawnRate;
    [SerializeField] GameObject zombiePrefab;


    void Start()
    {
        StartCoroutine(ISpawnZombies());
    }


    IEnumerator ISpawnZombies()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(zombieSpawnRate);
            Instantiate(zombiePrefab, transform.position, transform.rotation);
        }
        
    }


} // END Class
