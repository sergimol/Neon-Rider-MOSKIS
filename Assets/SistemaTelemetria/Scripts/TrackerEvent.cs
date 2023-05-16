using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class TrackerEvent
{
    //Variables comunes
    protected string type; 
    protected long timeStamp;

    public string choice;

    public TrackerEvent(string t)
    {
        type = t;
        timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
    }
    public TrackerEvent()
    {
    }

    public string GetEventType()
    {
        return type;
    }

    public long getTimeStamp()
    {
        return timeStamp;
    }

    // SERIALIZADORES
    public virtual string toJSON()
    {
        // Atributos comunes a todos los eventos
        string aux = "{\"TimeStamp\": \"" + timeStamp.ToString() + "\", \"SessionId\": \"" +
            Tracker.instance.getSessionId().ToString() + "\", \"EventType\": \"" + type + "\"";

        return aux;
    }
    public virtual string toCSV()
    {
        // Atributos comunes a todos los eventos
        string aux = "TimeStamp," + timeStamp + "," + "EventType," + type + "," + "SessionId," + Tracker.instance.getSessionId().ToString() + ",";

        return aux;
    }
    public virtual string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        // Escribir el elemento Evento (Su tipo)
        xml_writer.WriteStartElement(type);

        // Escribimos sus atributos (variables)
        xml_writer.WriteAttributeString("SessionId", Tracker.instance.getSessionId().ToString());
        xml_writer.WriteAttributeString("TimeStamp", timeStamp.ToString());
        

        return stringWriter.ToString();
    }

}
