using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerConfig : MonoBehaviour
{
    // Configuracion del tracker especifica para el juego
    // Struct y array para elegir desde el editor que eventos se trackean
    [Serializable]
    public struct EventConfig
    {
        public string eventName;
        public bool isTracked;
    }
    [SerializeField]
    public EventConfig[] eventConfig;

    // Diccionario para comprobar rapidamente si debe trackearse un evento durante ejecucion
    public Dictionary<string, bool> eventsTracked = new Dictionary<string, bool>();

    private void Awake()
    {
        foreach(var config in eventConfig)
        {
            eventsTracked.Add(config.eventName, config.isTracked);
        }
    }

    private void Start()
    {
        
    }

    public Dictionary<string, bool> getEvents() { return eventsTracked; }
}
