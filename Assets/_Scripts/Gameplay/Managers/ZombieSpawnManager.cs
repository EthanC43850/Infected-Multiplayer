using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{

    public static ZombieSpawnManager Instance;
    public List<Targetable> targets;

    private void Awake()
    {
        Instance = this;

    }

    public void AddTargetToList(Targetable target)
    {
        Debug.Log("Adding AI Target to list");
        targets.Add(target);

        Debug.Log("The list contains: " + targets);

    }


    public void RemoveTargetFromList(Targetable target)
    {



    }


}
