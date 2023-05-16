using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject canvasObject = Tracker.instance.GetComponent<GraphPersistence>().getChartCanvas();
        canvasObject.GetComponent<Canvas>().worldCamera = gameObject.GetComponent<Camera>();
    }
}
