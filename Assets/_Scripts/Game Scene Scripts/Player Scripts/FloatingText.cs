using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    //Origin point in canvas is (5, -136, -1)
    //[SerializeField] Transform floatingTextTransform;
    public Vector3 randomizeIntensity = new Vector3(50, 90, 0);

    private void Start()
    {
        //transform.localPosition += floatingTextTransform.localPosition;
        transform.localPosition += new Vector3(Random.Range(-randomizeIntensity.x, randomizeIntensity.x), Random.Range(0, randomizeIntensity.y), 0);


    }
}
