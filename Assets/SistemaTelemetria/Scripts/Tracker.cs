using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;

public class Tracker : MonoBehaviour
{
    public static Tracker instance = null;
    long sessionId;

    [SerializeField]    //Espaciado entre posts
    float timeBetweenPosts;
    float tSinceLastPost = 0;

    TrackerConfig config;
    FilePersistence filePersistence;
    [SerializeField] bool filePers = true;
    ServerPersistence serverPersistence;
    [SerializeField] bool serverPers = true;
    GraphPersistence graphPersistence;
    [SerializeField] bool graphPers = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.Init();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Init()
    {
        config = GetComponent<TrackerConfig>();
        filePersistence = GetComponent<FilePersistence>();
        serverPersistence = GetComponent<ServerPersistence>();
        graphPersistence = GetComponent<GraphPersistence>();

        sessionId = AnalyticsSessionInfo.sessionId;
    }
    private void Start()
    {
        AddEvent(new InicioEvent());
    }

    private void Update()
    {
        if (tSinceLastPost > timeBetweenPosts)
        {
            if (filePers) filePersistence.Flush();
            if (serverPers) serverPersistence.Flush();
            tSinceLastPost = 0;
        }
        else
            tSinceLastPost += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddEvent(new InicioEvent());
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddEvent(new FinEvent());
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            AddEvent(new FinEvent());
            if (filePers) filePersistence.Flush();
            if (serverPers) serverPersistence.Flush();
        }
    }

    public void AddEvent(TrackerEvent e)
    {
        string t = e.GetEventType();
        if (!config.eventsTracked.ContainsKey(t))
            Debug.Log("El evento " + t + " no se encuentra en la lista de eventos");
        else if (config.eventsTracked[t])
        {
            if (filePers) filePersistence.Send(e);
            if (serverPers) serverPersistence.Send(e);
            if (graphPers) graphPersistence.Send(e);
        }
    }

    public long getSessionId()
    {
        return sessionId;
    }
}
