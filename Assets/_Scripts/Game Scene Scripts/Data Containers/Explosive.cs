using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Explosive : Item
{
    public abstract override void Use();

    [Header("Attributes")]
    public GameObject explosionParticle;
    public GameObject radiusIndicator;

}
