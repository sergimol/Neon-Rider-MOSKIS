using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FinNivelEvent : TrackerEvent
{
    // Atributos del evento
    int levelId;

    public FinNivelEvent(int rId) : base("FinNivel")
    {
        levelId = rId;
    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"LevelID\": \"" + levelId.ToString() + "\"},";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "LevelID," + levelId.ToString() + ",";
        return cadena;
    }
    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("LevelId", levelId.ToString());

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();
        return stringWriter.ToString();
    }
}
