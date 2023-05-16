using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class FilePersistence : IPersistence
{
    List<TrackerEvent> eventsBuff;

    [SerializeField]
    bool serializeInJSON = true;
    [SerializeField]
    bool serializeInXML = true;
    [SerializeField]
    bool serializeInCSV = true;

    JSONSerializer serializerJSON = null;   //JSON
    StreamWriter jsonStream = null;

    XMLSerializer serializerXML = null;     //XML
    StreamWriter xmlStream = null;

    CSVSerializer serializerCSV = null;     //CSV
    StreamWriter csvStream = null;

    string ruta_base = "Trazas\\";
    string ruta_json = "JSON\\";
    string ruta_xml = "XML\\";
    string ruta_csv = "CSV\\";

    private void Start()
    {
        eventsBuff = new();
        string id = Tracker.instance.getSessionId().ToString();

        // Crear carpetas del tracker
        if (!Directory.Exists(ruta_base))
            Directory.CreateDirectory(ruta_base);
        if (!Directory.Exists(ruta_base + ruta_json))
            Directory.CreateDirectory(ruta_base + ruta_json);
        if (!Directory.Exists(ruta_base + ruta_xml))
            Directory.CreateDirectory(ruta_base + ruta_xml);
        if (!Directory.Exists(ruta_base + ruta_csv))
            Directory.CreateDirectory(ruta_base + ruta_csv);

        if (serializeInJSON)
        {
            serializerJSON = GetComponent<JSONSerializer>();
            jsonStream = new StreamWriter(ruta_base + ruta_json + id + ".json");
        }
        if (serializeInCSV)
        {
            serializerCSV = GetComponent<CSVSerializer>();
            csvStream = new StreamWriter(ruta_base + ruta_csv + id + ".csv");
        }
        if (serializeInXML)
        {
            serializerXML = GetComponent<XMLSerializer>();
            xmlStream = new StreamWriter(ruta_base + ruta_xml + id + ".xml");
        }
    }

    private void OnDestroy()
    {
        if (jsonStream != null) jsonStream.Close();
        if (xmlStream != null) xmlStream.Close();
        if (csvStream != null) csvStream.Close();
    }

    public override void Send(TrackerEvent e)
    {
        eventsBuff.Add(e);
    }

    public override void Flush()
    {
        List<TrackerEvent> events = new List<TrackerEvent>(eventsBuff);
        eventsBuff.Clear();
        Write(events);
    }

    private void Write(List<TrackerEvent> events)
    {
        foreach (TrackerEvent e in events)
        {
            if (serializeInJSON)
            {
                string ser = serializerJSON.Serialize(e);
                jsonStream.WriteLine(ser);
            }
            if (serializeInXML)
            {
                string ser = serializerXML.Serialize(e);
                xmlStream.WriteLine(ser);
            }
            if (serializeInCSV)
            {
                string ser = serializerCSV.Serialize(e);
                csvStream.WriteLine(ser);
            }
        }
    }
}
