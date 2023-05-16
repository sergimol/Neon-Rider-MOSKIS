using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;
using UnityEditor;
using System.Linq;
using static UnityEngine.GraphicsBuffer;
using System.Net.Security;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using UnityEditor.PackageManager.UI;

/*
 * GraphTypes define las diferentes formas de contar los eventos
 * 
 * ACCUMULATED: Se acumula la cantidad de veces que aparece el evento Y desde el inicio hasta el numero de veces que haya aparecido X
 * 
 * NOTACCUMULATED: Cada vez que aparece un evento X, se reinicia la cantidad de veces que ha aparecido el evento Y
 * 
 * AVERAGE: Cada vez que aparece el evento X se hace una media de las veces que ha aparecido el evento Y desde el inicio
 * 
 */
public enum GraphTypes { ACCUMULATED, NOTACCUMULATED, AVERAGE }

/*
 * Scaling define las diferentes formas en las que se escalan las lineas dentro de los charts
 * 
 * X_SCALING_START: La linea ocupa siempre todo el chart en X y con cada evento nuevo se estrecha para insertar el nuevo punto
 * 
 * X_SCALING_OFFSET: La linea empieza de la izquierda y avanza x_segments veces para llegar a ocupar todos los valores de X,
 *      luego se estrecha para insertar nuevos puntos
 *      
 * ONLY_Y: La linea empieza de la izquierda y avanza x_segments veces para llegar a ocupar todos los valores de X,
 *      luego se mueven todos los puntos a la izquierda cuando es necesario insertar uno nuevo,
 *      las distancias entre los puntos son siempre las mismas.
 * 
 */
public enum Scaling { X_SCALING_START, X_SCALING_OFFSET, ONLY_Y }

/*
 * Constraints define de que manera se van a colocar los charts dentro de la pantalla
 * 
 * FREE_CONFIG: Se deja al diseñador que mueva cada chart individualmente y lo escale como quiera,
 *      con las variables graph_X, graph_Y y scale
 *      
 * LEFT_TOP: El primer chart se coloca arriba a la izquierda y los siguientes se situan directamente a la derecha,
 *      cuando no haya espacio en la pantalla para mas se empezara debajo del primero y siguiendo a la derecha
 *      
 * LEFT_BOTTOM: El primer chart se coloca abajo a la izquierda y los siguientes a la derecha, la siguiente fila empieza encima del primero
 * 
 * LEFT_VERTICAL: El primer chart se coloca arriba a la izquierda y los siguientes se situan directamente abajo,
 *      cuando no haya espacio en la pantalla para mas se empezara a la derecha del primero y siguiendo abajo
 *      
 * RIGHT_VERTICAL: El primer chart se coloca arriba a la derecha y los siguientes abajo, la siguiente columna empieza a la izquierda del primero
 * 
 */
public enum Constrains { FREE_CONFIG, LEFT_TOP, LEFT_BOTTOM, LEFT_VERTICAL, RIGHT_VERTICAL }

[Serializable]
public struct GraphConfig
{
    [HideInInspector]
    public string name;
    [HideInInspector]
    public string eventX;
    [HideInInspector]
    public string eventY;
    [HideInInspector]
    public int pointsNumber;
    [HideInInspector]
    public AnimationCurve myCurve;
    [HideInInspector]
    public GameObject window_graph;
    //Config del window_graph
    [HideInInspector]
    public float graph_Height;
    [HideInInspector]
    public float graph_Width;
    //Posicion en X e Y
    [HideInInspector]
    public int graph_X;
    [HideInInspector]
    public int graph_Y;
    [HideInInspector]
    public float scale;
    [HideInInspector]
    public int x_segments; // numero de separaciones que tiene el Eje X (Ademas es el numero de puntos que se representan en la grafica a la vez)
    [HideInInspector]
    public int y_segments; // numero de separaciones que tiene el Eje Y
    [HideInInspector]
    public Scaling scaling;
    [HideInInspector]
    [Range(0.0f, 0.2f)]
    public float line_Width;
    [HideInInspector]
    [Range(0.0f, 1.0f)]
    public float point_Size;
    [HideInInspector]
    public GraphTypes graphType;
    [HideInInspector]
    public Color designerGraphCol;
    [HideInInspector]
    public Color actualGraphCol;
}

public class GraphPersistence : IPersistence
{
    public GameObject graphObject;

    public Constrains constrainsGraphs;

    //La configuracion inicial de todos los graphs desde el editor
    public GraphConfig[] graphsConfig;

    Window_Graph[] graphs;

    private string baseSaveRoute = "Trazas\\Graphs\\";
    private Dictionary<string, StreamWriter> graphWriters;

    float preset_Scale = 1f;

    Resolution resolution;
    public Vector2 dimension;
    int max_charts_per_row = 4;
    int max_charts_per_col = 3;

    private void Start()
    {
        graphWriters = new Dictionary<string, StreamWriter>();

        // Crear la carpeta donde se guuardaran los archivos que contienen los datos con los puntos de la grafica
        string id = Tracker.instance.getSessionId().ToString();
        string fullRoute = baseSaveRoute + id + "\\";
        Directory.CreateDirectory(fullRoute);

        // Crear un nuevo objeto Canvas
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<CanvasScaler>();              // Agregar el componente grafico CanvasScaler al objeto Canvas
        canvasObject.AddComponent<GraphicRaycaster>();          // Agregar el componente grafico GraphicRaycaster al objeto Canvas
        canvasObject.transform.SetParent(transform, false);     // Hacer que el objeto Canvas sea hijo del objeto padre
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        canvasObject.GetComponent<Canvas>().worldCamera = Camera.main;
        canvasObject.GetComponent<Canvas>().scaleFactor = 1f;


        // Resolucion
        CanvasScaler cScaler = canvasObject.GetComponent<CanvasScaler>();
        cScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        resolution = Screen.currentResolution;
        cScaler.referenceResolution = new Vector2(resolution.width, resolution.height);
        //Tamano
        dimension = new Vector2(Screen.width, Screen.height);

        //Crear tantos Graph como se han configurado y pasarles la informacion
        Array.Resize(ref graphs, graphsConfig.Count());
        int rowIndex = 0;
        int colIndex = 0;
        for (int i = 0; i < graphsConfig.Count(); ++i)
        {
            // Creamos el objeto grafica
            GameObject aux = Instantiate(graphObject, parent: canvasObject.transform);

            // Rescalamos y posicionamos 
            SetGraphInWindow(ref aux, i, rowIndex, colIndex);

            graphs[i] = aux.GetComponent<Window_Graph>();
            graphs[i].name = graphsConfig[i].name;
            graphs[i].SetConfig(graphsConfig[i]);

            // Crear el archivo en el que se guardaran los puntos en formato de texto
            graphWriters.Add(graphsConfig[i].name, new StreamWriter(fullRoute + graphsConfig[i].name + ".csv"));
            graphWriters[graphsConfig[i].name].WriteLine(graphsConfig[i].eventX + "," + graphsConfig[i].eventY);

            rowIndex++;
            if(rowIndex >= max_charts_per_row)
                rowIndex = 0;
            colIndex++;
            if (colIndex >= max_charts_per_col)
                colIndex = 0;
        }
    }

    private void FixedUpdate()
    {
        // En cada update se reescriben las lineas del chart porque se pintan sobre el espacio del juego
        for (int i = 0; i < graphsConfig.Count(); ++i)
        {
            graphs[i].RefreshChart();
        }
    }

    private void Update()
    {
        // Con la tecla Q se desactivan todos los charts
        if (Input.GetKeyDown(KeyCode.RightControl))
            transform.GetChild(0).transform.gameObject.SetActive(!transform.GetChild(0).transform.gameObject.activeSelf);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < graphs.Length; ++i)
        {
            graphWriters[graphs[i].name].Close();
        }
    }
    public override void Send(TrackerEvent e)
    {
        for (int i = 0; i < graphs.Length; ++i)
        {
            // Comprueba si la grafica tiene el evento y si debe mostrar un nuevo punto
            if (graphs[i].ReceiveEvent(e))
            {
                // Si muestra un nuevo punto lo escribe en archivo para guardarlo
                Vector2 pos = graphs[i].getLatestPoint();
                // Formato: X (de los dos puntos), Y (del punto de la grafica del jugador), Y (del punto de la grafica del disenador)
                graphWriters[graphs[i].name].WriteLine(pos.x + "," + pos.y + "," + graphs[i].getLatestObjectivePoint());
            }
        }
    }

    public override void Flush()
    {

    }

    // Ajusta la posicion y escala de la Grafica
    private void SetGraphInWindow( ref GameObject chart, int index, int rowIndex, int colIndex)
    {
        RectTransform rectChart = chart.GetComponent<RectTransform>();
        rectChart.localScale = new Vector3(preset_Scale / max_charts_per_row, preset_Scale / max_charts_per_row, preset_Scale / max_charts_per_row);

        float offsetX = 0;
        float offsetY = 0;
        int row = 0;
        int col = 0;

        switch (constrainsGraphs)
        {
            // HORIZONTAL ABAJO
            case Constrains.LEFT_BOTTOM:
                offsetX = (resolution.width / max_charts_per_row) * rowIndex;
                row = index / max_charts_per_row;
                offsetY = rectChart.anchoredPosition.y + row * (rectChart.rect.height / max_charts_per_row); // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constrains.LEFT_TOP:
                offsetX = (resolution.width / max_charts_per_row) * rowIndex;
                row = index / max_charts_per_row;
                offsetY = resolution.height - ((row+1) * (rectChart.rect.height / max_charts_per_row)); // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constrains.LEFT_VERTICAL:
                col = index / max_charts_per_col;
                offsetX = (resolution.width / max_charts_per_col) * col;
                offsetY = resolution.height - (1080 / max_charts_per_col) - (resolution.height / max_charts_per_col) * colIndex; // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constrains.RIGHT_VERTICAL:
                col = index / max_charts_per_col;
                offsetX = resolution.width - (rectChart.rect.width / max_charts_per_col) * (col + 1);
                offsetY = resolution.height - (1080 / max_charts_per_col) - (1080 / max_charts_per_col) * colIndex; // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constrains.FREE_CONFIG:
                rectChart.anchoredPosition = new Vector2(graphsConfig[index].graph_X, graphsConfig[index].graph_Y);
                rectChart.localScale = new Vector3(preset_Scale * graphsConfig[index].scale, preset_Scale * graphsConfig[index].scale, preset_Scale * graphsConfig[index].scale); break;
        }
    }

    public GameObject getChartCanvas()
    {
        return transform.GetChild(0).gameObject;
    }
}

[CustomEditor(typeof(GraphPersistence))]
public class GraphPersistenceEditor : Editor
{
    SerializedProperty grPers;

    void OnEnable()
    {
        grPers = serializedObject.FindProperty("graphsConfig");
    }

    void OnValidate()
    {

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(grPers);

        GraphPersistence graphPersistence = (GraphPersistence)target;

        //Obten los nombres de los eventos del script TrackerConfig
        TrackerConfig trackerConfig = graphPersistence.gameObject.GetComponent<TrackerConfig>();
        List<string> eventNames = new List<string>();
        foreach (TrackerConfig.EventConfig config in trackerConfig.eventConfig)
        {
            eventNames.Add(config.eventName);
        }

        //Metodo de checkeo de cambios en el editor
        EditorGUI.BeginChangeCheck();

        graphPersistence.constrainsGraphs = (Constrains)EditorGUILayout.EnumPopup("Constrains", graphPersistence.constrainsGraphs);
        EditorGUILayout.Space(10);

        // Crea un menu dropdown para cada elemento de graphs
        for (int i = 0; i < graphPersistence.graphsConfig.Length; i++)
        {
            GraphConfig actGraphConf = graphPersistence.graphsConfig[i];
            actGraphConf.name = EditorGUILayout.TextField("GraphName", actGraphConf.name);
            if (actGraphConf.name == "" || (i-1 >=0 && actGraphConf.name == graphPersistence.graphsConfig[i-1].name))
                actGraphConf.name = "Chart" + i;

            actGraphConf.pointsNumber = EditorGUILayout.IntField("NumberPoints", actGraphConf.pointsNumber);
            if (actGraphConf.pointsNumber < 1)
                actGraphConf.pointsNumber = 1;

            actGraphConf.myCurve = EditorGUILayout.CurveField(actGraphConf.myCurve);
            EditorGUILayout.Space(5);

            //Variables de escala
            actGraphConf.graphType = (GraphTypes)EditorGUILayout.EnumPopup("GraphType", actGraphConf.graphType);
            actGraphConf.scaling = (Scaling)EditorGUILayout.EnumPopup("Scaling", actGraphConf.scaling);

            // Crea un menu popup con los nombres de los eventos
            int selectedEventIndex;
            selectedEventIndex = EditorGUILayout.Popup("Select Event X", eventNames.IndexOf(actGraphConf.eventX), eventNames.ToArray());
            if (selectedEventIndex != -1 && actGraphConf.eventX != eventNames[selectedEventIndex])
                actGraphConf.eventX = eventNames[selectedEventIndex];

            selectedEventIndex = EditorGUILayout.Popup("Select Event Y", eventNames.IndexOf(actGraphConf.eventY), eventNames.ToArray());
            if (selectedEventIndex != -1 && actGraphConf.eventY != eventNames[selectedEventIndex])
                actGraphConf.eventY = eventNames[selectedEventIndex];


            //El resto de configuracion
            actGraphConf.line_Width = EditorGUILayout.Slider("Line Width", actGraphConf.line_Width, 0.01f, 0.2f);
            actGraphConf.point_Size = EditorGUILayout.Slider("Point Size", actGraphConf.point_Size, 0.01f, 1.0f);

            if (graphPersistence.constrainsGraphs == Constrains.FREE_CONFIG)
            {
                actGraphConf.graph_X = EditorGUILayout.IntSlider("X Pos", actGraphConf.graph_X, 0, Screen.currentResolution.width);
                actGraphConf.graph_Y = EditorGUILayout.IntSlider("Y Pos", actGraphConf.graph_Y, 0, Screen.currentResolution.height);
                actGraphConf.scale = EditorGUILayout.Slider("Scale", actGraphConf.scale, 0.01f, 1.0f);
            }

            actGraphConf.x_segments = EditorGUILayout.IntField("X segments", actGraphConf.x_segments);
            if (actGraphConf.x_segments < 2)
                actGraphConf.x_segments = 2;
            actGraphConf.y_segments = EditorGUILayout.IntField("Y segments", actGraphConf.y_segments);
            if (actGraphConf.y_segments < 2)
                actGraphConf.y_segments = 2;

            actGraphConf.designerGraphCol = EditorGUILayout.ColorField("DesignerGraph", actGraphConf.designerGraphCol);
            actGraphConf.designerGraphCol.a = 1;
            actGraphConf.actualGraphCol = EditorGUILayout.ColorField("ActualGraph", actGraphConf.actualGraphCol);
            actGraphConf.actualGraphCol.a = 1;

            EditorGUILayout.Space(20);
            graphPersistence.graphsConfig[i] = actGraphConf;
        }
        EditorGUILayout.Space();
        graphPersistence.graphObject = EditorGUILayout.ObjectField("Graph Object", graphPersistence.graphObject, typeof(GameObject), false) as GameObject;

        //Si ha habido cambios utilizamos setDirty para que unity no cambie los valores de editor y se mantengan para ejecucion
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);

        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();
    }
}
