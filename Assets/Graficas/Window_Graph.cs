using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEditor.PlayerSettings;

public class Window_Graph : MonoBehaviour
{
    [SerializeField]
    Sprite circle_sprite;
    float circle_scale = 60;

    // Puntos de la telemetria
    List<float> points;
    [SerializeField]
    LineRenderer line_renderer;

    // Puntos del disenador
    List<float> objective_points;
    [SerializeField]
    LineRenderer objective_line_renderer;

    // Containers para los objetos
    [SerializeField]
    RectTransform graph_container;
    [SerializeField]
    GameObject points_container;
    [SerializeField]
    GameObject lines_container;
    [SerializeField]
    GameObject label_X_container;
    [SerializeField]
    GameObject label_Y_container;
    [SerializeField]
    RectTransform left_container;

    // Objetos que generamos
    [SerializeField]
    RectTransform label_template_X;
    [SerializeField]
    RectTransform label_template_Y;
    [SerializeField]
    RectTransform dash_template_X;
    [SerializeField]
    RectTransform dash_template_Y;

    // Leyenda
    [SerializeField]
    Image obj_image;
    [SerializeField]
    Image track_image;
    [SerializeField]
    TextMeshProUGUI chart_name;



    float x_size; // distancia entre puntos de X
    float y_size; // distancia entre puntos de Y
    float x_pos = 0;

    // Configuracion del grafico
    GraphConfig graphConfig;
    /*ESTO ESTA DENTRO DE GRAPHCONFIG
    // Dimensiones del Grafico
    //public float graph_Height;
    //public float graph_Width;
    //public int x_segments = 10; // numero de separaciones que tiene el Eje X (Ademas es el numero de puntos que se representan en la grafica a la vez)
    //public int y_segments = 8; // numero de separaciones que tiene el Eje Y
    // |-------------*-
    // |-------*------- 
    // |----*-----*----
    // |-*-------------
    // +---------------
    // Los puntos se van ocultando por la izquierda <- <- <-
    */

    float x_max; // Valor maximo que tiene el Eje X en cada momento
                 // Cada vez que se anade un punto se suma
    float y_max; // Valor maximo que puede alcanzar el Eje Y 
                 // Inicialmente lo determina el mayor valor de la grafica del disenador
                 // Posteriormente se actualiza si aparecen valores mayores

    // Contador para colocar el siguiente punto generado por eventos
    float nextY = 0;

    // Listas de objetos generados
    TextMeshProUGUI[] label_Y_List;
    List<TextMeshProUGUI> label_X_List;
    List<GameObject> circles;
    List<GameObject> objective_circles;
    int objective_index = 0;


    // Renderizado dentro del Viewport
    [SerializeField]
    ScrollRect render_viewport;


    public void SetConfig(GraphConfig g)
    {
        graphConfig = g;
        label_Y_List = new TextMeshProUGUI[graphConfig.y_segments + 1];

        // Altura del grid, altura base 5
        graphConfig.graph_Height = graph_container.sizeDelta.y;
        // Ancho del grid, anchuira base 5
        graphConfig.graph_Width = graph_container.sizeDelta.x;

        // Listas con lo necesario para pintar las lineas
        circles = new List<GameObject>();
        objective_circles = new List<GameObject>();
        points = new List<float>();

        // Lista para los numeros del eje X, es necesario que sea dinámico
        label_X_List = new List<TextMeshProUGUI>();

        // El maximo del eje X al principio es el numero de segmentos, si se mueve a la derecha se actualizara
        x_max = graphConfig.x_segments;

        List<float> objectiveLineAux = new List<float>();

        // Evaluate para sacar los valores del AnimGraph dado por el disenador
        for (int i = 0; i < graphConfig.myCurve.length; ++i)
        {
            float aux = graphConfig.myCurve.Evaluate(i);
            objectiveLineAux.Add(aux);
        }
        SetObjectiveLine(objectiveLineAux);
        y_max = getMaxFromList();

        // Se enseña el chart vacio por pantalla
        ShowGraph();

        // Setteo de los tamaños de las lineas y puntos
        line_renderer.startWidth = graphConfig.line_Width;
        line_renderer.endWidth = graphConfig.line_Width;
        objective_line_renderer.startWidth = graphConfig.line_Width;
        objective_line_renderer.endWidth = graphConfig.line_Width;


        // Obtener el tamano de la resolucion actual de la pantalla
        Resolution resolution = Screen.currentResolution;


        // Obtener el tamano de la ventana de juego
        int windowWidth = Screen.width;
        int windowHeight = Screen.height;

        circle_scale *= graphConfig.point_Size;


        // Nombre y Leyenda
        List<Material> m = new List<Material>();
        line_renderer.GetMaterials(m);
        m[0].color = graphConfig.actualGraphCol;
        track_image.color = m[0].color;

        List<Material> m2 = new List<Material>();
        objective_line_renderer.GetMaterials(m2);
        m2[0].color = graphConfig.designerGraphCol;
        obj_image.color = m2[0].color;
        chart_name.text = g.name;

    }

    void ShowGraph()
    {
        x_size = graphConfig.graph_Width / graphConfig.x_segments;
        y_size = graphConfig.graph_Height / graphConfig.y_segments;

        float x = 0;
        float y = x;

        // MARCADORES EJE X - - - - - - - -
        for (int i = 0; i < graphConfig.x_segments + 1; i++)
        {
            RectTransform dashX = Instantiate(dash_template_X, lines_container.transform);
            dashX.anchoredPosition = new Vector2(x, 0);
            // se escala la terxtura en el eje X hasta que cubra el chart entero
            dashX.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphConfig.graph_Height);

            RectTransform labelX = Instantiate(label_template_X, label_X_container.transform);
            labelX.anchoredPosition = new Vector2(x, 0);
            labelX.GetComponent<TextMeshProUGUI>().text = i.ToString("F1"); // F1 hace que se quede solo con 1 decimal para evitar floats grandes
            label_X_List.Add(labelX.GetComponent<TextMeshProUGUI>());

            x += x_size;
        }

        // MARCADORES EJE Y
        for (int i = 0; i < graphConfig.y_segments + 1; i++)
        {

            RectTransform dashY = Instantiate(dash_template_Y, lines_container.transform);
            dashY.anchoredPosition = new Vector2(0, y);
            dashY.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphConfig.graph_Width);

            RectTransform labelY = Instantiate(label_template_Y, label_Y_container.transform);
            labelY.anchoredPosition = new Vector2(0, y);
            labelY.GetComponent<TextMeshProUGUI>().text = ((y_max / graphConfig.y_segments) * i).ToString("F1");

            y += y_size;

            label_Y_List[i] = labelY.GetComponent<TextMeshProUGUI>();
        }
    }

    // Anade un nuevo punto a la grafica
    public void AddPoint(float new_y)
    {

        /// PUNTO TELEMETRIA
        // Anadimos el nuevo punto
        points.Add(new_y);

        // Lo creamos
        float y_pos = (points[points.Count - 1] / y_max) * graphConfig.graph_Height;
        GameObject point_object = CreateCircle(new Vector2(x_pos, y_pos));
        circles.Add(point_object);


        /// PUNTO OBJETIVO
        // Anadimos el nuevo punto
        if (objective_points.Count >= points.Count)
        {
            // Lo creamos
            float o_y_pos = (objective_points[objective_index] / y_max) * graphConfig.graph_Height;
            GameObject o_point_object = CreateCircle(new Vector2(x_pos, o_y_pos));
            objective_circles.Add(o_point_object);
        }

        // Se hace el check de los dos puntos a la vez para evitar reescalar dos veces
        CheckMove(new Vector2(objective_index, new_y), new Vector2(points.Count - 1, new_y));

        x_pos += x_size;

        if (objective_points.Count >= points.Count)
            objective_index++;

        // Lo unimos a la grafica
        CreateLine();
    }

    // Crea la linea que une todos los puntos
    // Solo dibuja la linea entre los puntos que se renderizan dentro del viewport
    private void CreateLine()
    {
        /// TELEMETRIA ///
        // Creamos una lista de Posiciones dentro del Viewport
        List<Vector3> aux = new List<Vector3>();
        for (int i = 0; i < circles.Count; i++)
        {
            Transform t = circles[i].transform;

            // Comprobamos que el punto se este renderizando en el Viewport
            if (RectTransformUtility.RectangleContainsScreenPoint(render_viewport.viewport, t.position))
            {
                Vector3 v = new Vector3(t.position.x, t.position.y);
                aux.Add(v);
            }
        }
        // Volcamos la lista en un vector
        Vector3[] aux_def = new Vector3[aux.Count];
        for (int i = 0; i < aux.Count; i++)
        {
            aux_def[i] = aux[i];
        }


        // Creamos la linea 
        line_renderer.positionCount = aux.Count;
        line_renderer.SetPositions(aux_def);

        /// OBJETIVO ///
        // Creamos una lista de Posiciones dentro del Viewport
        List<Vector3> aux_o = new List<Vector3>();
        for (int i = 0; i < objective_index; i++)
        {
            Transform t = objective_circles[i].transform;

            // Comprobamos que el punto se este renderizando en el Viewport
            if (RectTransformUtility.RectangleContainsScreenPoint(render_viewport.viewport, t.transform.position))
            {
                Vector3 v = new Vector3(t.position.x, t.position.y);
                aux_o.Add(v);
            }
        }
        // Volcamos la lista en un vector
        Vector3[] aux_def_o = new Vector3[aux_o.Count];
        for (int i = 0; i < aux_o.Count; i++)
        {
            aux_def_o[i] = aux_o[i];
        }

        // Creamos la linea 
        objective_line_renderer.positionCount = aux_o.Count;
        objective_line_renderer.SetPositions(aux_def_o);
    }

    public void RefreshChart()
    {
        CreateLine();
    }

    // Desplaza la Grafica a la Izquierda 1 posicion
    private void MoveLeft()
    {
        // Movemos el container
        left_container.anchoredPosition = new Vector2(left_container.anchoredPosition.x - x_size, left_container.anchoredPosition.y);

        // Anadimos el nuevo marcador abajo
        RectTransform labelX = Instantiate(label_template_X, label_X_container.transform);
        labelX.anchoredPosition = new Vector2(x_pos, 0);
        labelX.GetComponent<TextMeshProUGUI>().text = label_X_List.Count.ToString("F1");
        label_X_List.Add(labelX.GetComponent<TextMeshProUGUI>());
    }

    private void CheckMove(Vector2 newPoint, Vector2 newPoint2)
    {
        if (graphConfig.scaling != Scaling.ONLY_Y)
        {
            if (newPoint.y > y_max)
                y_max = newPoint.y;
            if (newPoint2.y > y_max)
                y_max = newPoint2.y;
            // Si el escalado en X esta activo se debe reescalar en cada nuevo punto anadido
            ReScalePoints();
        }
        else
        {
            // Si el nuevo punto es mayor que el maximo que habia, ReEscalamos
            if (newPoint.y > y_max || newPoint2.y > y_max)
            {
                // y_max se quedara con la y mas grande de entre los dos puntos si hay al menos uno que supera la y_max anterior
                y_max = newPoint.y > newPoint2.y ? newPoint.y : newPoint2.y;
                ReScalePoints();
            }

            // Si Anadimos un nuevo punto y hay que desplazar la Grafica
            if (newPoint.x > x_max || newPoint2.x > x_max)
            {
                // x_max sera el mas grande de los dos puntos
                // Normalmente iran a la vez
                x_max = newPoint.x > newPoint2.x ? newPoint.x : newPoint2.x;
                MoveLeft();
            }
        }
    }

    // Crea un circulo, para representar graficamente un punto
    GameObject CreateCircle(Vector2 pos)
    {
        // Creamos una Imagen
        GameObject game_Object = new GameObject("Point");
        game_Object.AddComponent<Image>();
        game_Object.transform.SetParent(points_container.transform, false);

        // Seteamos la imagen 
        game_Object.GetComponent<Image>().sprite = circle_sprite;

        // Seteamos su posicion
        RectTransform rect = game_Object.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(circle_scale, circle_scale);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);

        return game_Object;
    }

    private float getMaxFromList()
    {
        return objective_points.Max();
    }

    // Re Escalamos los puntos para que se ajusten a los nuevos valores maximos del eje Y
    private void ReScalePoints()
    {
        // Escalado de los eventos del jugador
        for (int i = 0; i < circles.Count; i++)
        {
            RectTransform rect = circles[i].GetComponent<RectTransform>();
            float y_pos = (points[i] / y_max) * graphConfig.graph_Height;

            // Si esta activo el escalado en x el nuevo vector debera calcular la nueva posicion en X tambien
            // Si esta activo el escalado con offset, se espera hasta llegar a los segmentos de X
            if (graphConfig.scaling == Scaling.X_SCALING_START || (graphConfig.scaling == Scaling.X_SCALING_OFFSET && circles.Count > graphConfig.x_segments))
            {
                float x_pos = ((float)i / (circles.Count - 1)) * graphConfig.graph_Width;
                Vector2 pos = new Vector2(x_pos, y_pos);
                rect.anchoredPosition = pos;
            }
            else
            {
                Vector2 pos = new Vector2(rect.anchoredPosition.x, y_pos);
                rect.anchoredPosition = pos;
            }
        }

        // Escalado de los eventos del disenador, se toma circles.Count para que siempre coincidan con su valor en el eje X
        for (int i = 0; i < circles.Count; i++)
        {
            if (i < objective_circles.Count)
            {
                RectTransform rect = objective_circles[i].GetComponent<RectTransform>();
                float y_pos = (objective_points[i] / y_max) * graphConfig.graph_Height;

                if (graphConfig.scaling == Scaling.X_SCALING_START || (graphConfig.scaling == Scaling.X_SCALING_OFFSET && circles.Count > graphConfig.x_segments))
                {
                    float x_pos = ((float)i / (circles.Count - 1)) * graphConfig.graph_Width;
                    Vector2 pos = new Vector2(x_pos, y_pos);
                    rect.anchoredPosition = pos;
                }
                else
                {
                    Vector2 pos = new Vector2(rect.anchoredPosition.x, y_pos);
                    rect.anchoredPosition = pos;
                }
            }
        }

        // Cambio en el texto de los segmentos en Y
        for (int i = 0; i < label_Y_List.Length; i++)
        {
            label_Y_List[i].text = ((y_max / graphConfig.y_segments) * i).ToString("F1"); // F1 hace que se quede solo con 1 decimal para evitar floats grandes
        }

        if (graphConfig.scaling == Scaling.X_SCALING_START || (graphConfig.scaling == Scaling.X_SCALING_OFFSET && circles.Count > graphConfig.x_segments))
        {
            // Reescalado de los puntos en X que se actualiza cuando se anaden
            for (int i = 0; i < label_X_List.Count; i++)
            {
                label_X_List[i].text = (((float)(circles.Count - 1) / (float)graphConfig.x_segments) * i).ToString("F1");
            }
        }

    }

    // Inicializa la lista de puntos del Disenador (Llamar desde la persistencia al crear)
    public void SetObjectiveLine(List<float> o)
    {
        objective_points = new List<float>(o);
    }

    // Recibe un evento desde el sistema de persistencia y lo procesa si es necesario. Devuelve true solo si escribe un nuevo punto en la grafica
    public bool ReceiveEvent(TrackerEvent e)
    {
        string eventType = e.GetEventType();
        // Si el tipo del evento es igual que el del eje Y aumenta el contador que coloca el siguiente punto en Y
        if (eventType == graphConfig.eventY)
            nextY++;
        // Si el tipo del evento es igual que el del eje X coloca el siguiente punto teniendo en cuenta el tipo de grafica
        else if (eventType == graphConfig.eventX)
        {
            ProcessXEvent();
            return true;
        }
        return false;
    }

    private void ProcessXEvent()
    {
        switch (graphConfig.graphType)
        {
            case GraphTypes.AVERAGE:
                AddPoint(nextY / (points.Count + 1));
                break;
            case GraphTypes.NOTACCUMULATED:
                AddPoint(nextY);
                nextY = 0;
                break;
            default:
                AddPoint(nextY);
                break;
        }
    }

    public Vector2 getLatestPoint()
    {
        return new Vector2(objective_index - 1, points[^1]);
    }

    public float getLatestObjectivePoint()
    {
        return objective_points[objective_index - 1];
    }
}
