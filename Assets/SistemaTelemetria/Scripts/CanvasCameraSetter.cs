using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    // Script para cambiar la c�mara asignada al canvas de las gr�ficas al cambiar de nivel
    void Start()
    {
        GameObject canvasObject = Tracker.instance.GetComponent<GraphPersistence>().getChartCanvas();
        canvasObject.GetComponent<Canvas>().worldCamera = gameObject.GetComponent<Camera>();
    }
}
