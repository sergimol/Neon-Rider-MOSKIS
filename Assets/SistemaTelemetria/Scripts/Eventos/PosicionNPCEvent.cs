using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class PosicionNPCXML : DataXML
{
    public string x_pos;
}

public class PosicionNPCEvent : TrackerEvent
{
    // Atributos del evento
    Vector2 pos;
    int npcId;

    public PosicionNPCEvent(Vector2 p, int id): base("PosicionNPC")
    {
        pos = p;
        npcId = id;
    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"Pos\": \"" + pos.ToString() + "\"" 
               + ", \"NPCId\": \"" + npcId.ToString() + "\"},";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "Pos," + "\"" + pos.x.ToString() + "\"" + "," + "\"" + pos.y.ToString() + "\"" + ",";
        return cadena;
    }

    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("Pos", pos.ToString());

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();

        return stringWriter.ToString();
    }
}
