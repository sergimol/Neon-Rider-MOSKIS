using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// Este Script Dibuja el Grid de la Grafica
public class UiLineRenderer : Graphic
{
    [SerializeField]
    UiGridRenderer grid;

    [SerializeField]
    float thickness;

    [SerializeField]
    List<Vector2> points;

    float unitWidth;
    float unitHeight;
    float width;
    float height;

    Vector2Int lastGridSize = new Vector2Int(10, 10);

    // Metodo que se usa para redefinir mallas en ejecucion
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // VertexHelper es el objeto que permite redefinir una malla
        // Limpiamos los posibles vertices
        vh.Clear();

        lastGridSize = grid.getGridSize();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        unitWidth = width / (float)lastGridSize.x;
        unitHeight = height / (float)lastGridSize.y;

        if(points.Count < 2 ) { return; }

        // Recorremos todos los puntos
        for(int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            float angle = 0;
            if(i > points.Count - 1)
                angle = GetAngle(points[i], points[i + 1]) + 45;
            DrawVerticesForPoint(point, vh, angle);
        }

        // Recorremos todos los puntos
        for (int i = 0; i < points.Count-1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }


    }

    private void DrawVerticesForPoint(Vector2 point, VertexHelper vh, float angle)
    {


        // Definimos un vertice, lo reutilizamos todo el rato cambiando sus parametros
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        // Primer vertice
        vertex.position = Quaternion.Euler(0,0, angle) * new Vector2(-thickness/2, 0);
        vertex.position += new Vector3(unitWidth * point.x , unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector2(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

    }

    private void Update()
    {
        if(grid)
        {
            if(grid.getGridSize() != lastGridSize)
            {
                SetVerticesDirty();
            }
        }
    }

    public float GetAngle(Vector2 point1, Vector2 point2)
    {
        Vector2 direction = point2 - point1;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
}

