using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Este Script Dibuja el Grid de la Grafica
public class UiGridRenderer : Graphic
{
    [SerializeField]
    float thickness;

    [SerializeField]
    Vector2Int gridSize = new Vector2Int(10, 10);

    float cellWidth;
    float cellHeight;
    float width;
    float height;

    // Metodo que se usa para redefinir mallas en ejecucion
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // VertexHelper es el objeto que permite redefinir una malla
        // Limpiamos los posibles vertices
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        cellWidth = width / (float)gridSize.x;
        cellHeight = height / (float)gridSize.y;

        int count = 0; 
        for(int y = 0; y < gridSize.y; y++) 
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                DrawCell(x, y, count, vh);
                count++;
            }
        }
    }

    private void DrawCell(int x, int y, int index, VertexHelper vh)
    {
        float xPos = x * cellWidth;
        float yPos = y * cellHeight;

        // Definimos un vertice, lo reutilizamos todo el rato cambiando sus parametros
        UIVertex vertex = UIVertex.simpleVert;

        // Cuadrado
        vertex.position = new Vector2(xPos, yPos);
        vh.AddVert(vertex);

        vertex.position = new Vector2(xPos, yPos + cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector2(xPos + cellWidth, yPos + cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector2(xPos + cellWidth, yPos);
        vh.AddVert(vertex);


        // Cuadrado de dentro
        float withSquare = thickness * thickness;
        float distanceSquare = withSquare / 2;
        float distance = Mathf.Sqrt(distanceSquare);

        vertex.position = new Vector2(xPos + distance, yPos + distance);
        vh.AddVert(vertex);

        vertex.position = new Vector2(xPos + distance, yPos + (cellHeight - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector2(xPos + (cellWidth - distance), yPos + (cellHeight - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector2( xPos + (cellWidth - distance), yPos + distance);
        vh.AddVert(vertex);

        // Añadimos un offset de 8 triangulos (una celda)
        int offset = index * 8;

        // 
        vh.AddTriangle(offset + 0, offset + 1, offset + 5);
        vh.AddTriangle(offset + 5, offset + 4, offset + 0);
        // 
        vh.AddTriangle(offset + 1, offset + 2, offset + 6);
        vh.AddTriangle(offset + 6, offset + 5, offset + 1);
        // 
        vh.AddTriangle(offset + 2, offset + 3, offset + 7);
        vh.AddTriangle(offset + 7, offset + 6, offset + 2);
        // 
        vh.AddTriangle(offset + 3, offset + 0, offset + 4);
        vh.AddTriangle(offset + 4, offset + 7, offset + 3);
    }

    public Vector2Int getGridSize()
    {
        return gridSize;
    }
}

