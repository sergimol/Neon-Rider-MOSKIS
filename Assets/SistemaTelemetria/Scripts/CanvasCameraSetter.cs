using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    // Script para cambiar la cámara asignada al canvas de las gráficas al cambiar de nivel
    void Start()
    {
        GameObject canvasObject = Tracker.instance.GetComponent<GraphPersistence>().getChartCanvas();
        canvasObject.GetComponent<Canvas>().worldCamera = gameObject.GetComponent<Camera>();
    }
}
