using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirstrikeLineRenderer : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Vector3 height;

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, transform.position + height);
        lineRenderer.SetPosition(1, transform.position);

    }
}
