using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class MuerteEnemigoXML : DataXML
{
    public string x_pos;
    public string x_enemyId;
}

public class MuerteEnemigoEvent : TrackerEvent
{
    // Atributos del evento
    Vector2 pos;
    int enemyId;

    public MuerteEnemigoEvent(Vector2 p, int enId) : base("MuerteEnemigo")
    {
        pos = p;
        enemyId = enId;
    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"EnemyID\": \"" + enemyId.ToString() + "\"";
        cadena += ", \"Pos\": \"" + pos.ToString() + "\"},";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "EnemyID," + enemyId.ToString() + ",";
        cadena += "Pos," + "\"" + pos.x.ToString() + "\"" + "," + "\"" + pos.y.ToString() + "\"" + ",";
        return cadena;
    }

    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("Pos", pos.ToString());
        xml_writer.WriteAttributeString("EnemyId", enemyId.ToString());

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();
        return stringWriter.ToString();
    }
}
