using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;


public class FinEvent : TrackerEvent
{
    public FinEvent() : base("Fin")
    {

    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON() + "}]";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        return cadena;
    }

    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.WriteEndElement(); // Cerramos tambien la lista de Eventos
        xml_writer.Flush();
        return stringWriter.ToString();
    }
}
