using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


public class XMLSerializer : ISerializer
{

    XmlWriterSettings xml_settings = null;
    StringWriter xml_stringWriter = null;
    XmlWriter xml_writer = null;

    private void Start()
    {
        // Configuracion del formateador 
        xml_settings = new XmlWriterSettings();
        xml_settings.Indent = true;
        xml_settings.OmitXmlDeclaration = true;
        xml_settings.NewLineOnAttributes = true;

        xml_stringWriter = new StringWriter();
        xml_writer = XmlWriter.Create(xml_stringWriter, xml_settings);

        // Escribrimos el inicio del formato (Raiz)
        xml_writer.WriteStartElement("Eventos");

    }

    public override string Serialize(TrackerEvent e)
    {
        // Limpiamos el buffer auxiliar
        xml_stringWriter.GetStringBuilder().Clear();

        string cadena = e.toXML(ref xml_writer, ref xml_stringWriter);

        // La devolvemos como string
        return cadena;
    }
}
