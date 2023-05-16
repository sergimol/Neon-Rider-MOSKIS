using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour
{
    [SerializeField]
    List<Vector2> puntos;
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    GameObject o1;
    [SerializeField]
    GameObject o2;
    [SerializeField]
    Canvas ca;

    void Start()
    {

        sandokan();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void sandokan()
    {
        //lineRenderer.positionCount = puntos.Count;
        //for(int i = 0; i < puntos.Count; i++)
        //{
        //    lineRenderer.SetPosition(i, puntos[i]);
        //}
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, o1.transform.position);
        lineRenderer.SetPosition(1, o2.transform.position);

    }
}
