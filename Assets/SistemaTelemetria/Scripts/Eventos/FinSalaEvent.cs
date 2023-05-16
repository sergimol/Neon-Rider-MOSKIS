using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FinSalaXML : DataXML
{
    public string x_roomId;
}

public class FinSalaEvent : TrackerEvent
{
    // Atributos del evento
    int roomId;

    public FinSalaEvent(int rId) : base("FinSala")
    {
        roomId = rId;
    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"RoomId\": \"" + roomId.ToString() + "\"},";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "RoomID," + roomId.ToString() + ",";
        return cadena;
    }
    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("RoomId", roomId.ToString());

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();

        return stringWriter.ToString();
    }
}
