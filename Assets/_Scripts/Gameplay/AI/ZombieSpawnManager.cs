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


    public static void RemoveTargetFromList(Targetable target)
    {



    }


}
