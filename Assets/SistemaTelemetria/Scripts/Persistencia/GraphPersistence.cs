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

public enum GraphTypes { ACCUMULATED, NOTACCUMULATED, AVERAGE }
public enum Scaling { X_SCALING_START, X_SCALING_OFFSET, ONLY_Y }
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
    int max_charts_per_row = 4;
    int max_charts_per_col = 4;

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
        canvasObject.GetComponent<Canvas>().scaleFactor = 1f;  //!CUIDAO


        // ESTO ESTA SIENDO DELICADO
        CanvasScaler mivieja = canvasObject.GetComponent<CanvasScaler>();
        mivieja.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        resolution = Screen.currentResolution;
        mivieja.referenceResolution = new Vector2(resolution.width, resolution.height);

        //Crear tantos Graph como se han configurado y pasarles la informacion
        Array.Resize(ref graphs, graphsConfig.Count());
        int cuadIndex = 0;
        for (int i = 0; i < graphsConfig.Count(); ++i)
        {
            // Creamos el objeto grafica
            GameObject aux = Instantiate(graphObject, parent: canvasObject.transform);

            // Rescalamos y posicionamos 
            SetGraphInWindow(ref aux, i, cuadIndex);

            graphs[i] = aux.GetComponent<Window_Graph>();
            graphs[i].name = graphsConfig[i].name;
            graphs[i].SetConfig(graphsConfig[i]);

            // Crear el archivo en el que se guardaran los puntos en formato de texto
            graphWriters.Add(graphsConfig[i].name, new StreamWriter(fullRoute + graphsConfig[i].name + ".csv"));
            graphWriters[graphsConfig[i].name].WriteLine(graphsConfig[i].eventX + "," + graphsConfig[i].eventY);

            cuadIndex++;
            if(cuadIndex >= max_charts_per_row)
                cuadIndex = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            transform.GetChild(0).transform.gameObject.SetActive(!transform.GetChild(0).transform.gameObject.activeSelf);
    }

    private void OnDestroy()
    {
        if (graphs != null)
        {
            for (int i = 0; i < graphs.Length; ++i)
            {
                graphWriters[graphs[i].name].Close();
            }
        }
    }
    public override void Send(TrackerEvent e)
    {
        //eventsBuff.Add(e);
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
    private void SetGraphInWindow( ref GameObject chart, int index, int cuadIndex)
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
                float aux_offset = 0;
                if (index >= 4) aux_offset = 50;
                offsetX = (resolution.width / max_charts_per_row) * cuadIndex;
                row = index / max_charts_per_row;
                offsetY = rectChart.anchoredPosition.y + row * (rectChart.rect.height / max_charts_per_row); // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constrains.LEFT_TOP:
                offsetX = (resolution.width / max_charts_per_row) * cuadIndex;
                row = index / max_charts_per_row;
                // este 1080 hay que sacarlo a una variable porque el height del graph no es exacto y no da pa las cuentas
                offsetY = resolution.height - ((row+1) * (1080 / max_charts_per_row)); // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constrains.LEFT_VERTICAL:
                col = index / max_charts_per_col;
                offsetX = (1920 / max_charts_per_col) * col;
                offsetY = resolution.height - (1080 / max_charts_per_col) - (resolution.height / max_charts_per_col) * cuadIndex; // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constrains.RIGHT_VERTICAL:
                col = index / max_charts_per_col;
                offsetX = resolution.width - (1920 / max_charts_per_col) * (col + 1);
                offsetY = resolution.height - (1080 / max_charts_per_col) - (resolution.height / max_charts_per_col) * cuadIndex; // el height es el original por eso hay que reescalarlo para abajo
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
        //EditorUtility.SetDirty(target);
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
            actGraphConf.line_Width = EditorGUILayout.Slider("Line Width", actGraphConf.line_Width,0.0f,0.2f);
            actGraphConf.point_Size = EditorGUILayout.Slider("Point Size", actGraphConf.point_Size, 0.0f, 1.0f);

            if (graphPersistence.constrainsGraphs == Constrains.FREE_CONFIG)
            {
                actGraphConf.graph_X = EditorGUILayout.IntField("X Pos", actGraphConf.graph_X);
                actGraphConf.graph_Y = EditorGUILayout.IntField("Y Pos", actGraphConf.graph_Y);
                actGraphConf.scale = EditorGUILayout.FloatField("Scale", actGraphConf.scale);
            }

            actGraphConf.x_segments = EditorGUILayout.IntField("X segments", actGraphConf.x_segments);
            actGraphConf.y_segments = EditorGUILayout.IntField("Y segments", actGraphConf.y_segments);

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
