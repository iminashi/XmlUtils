using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlUtils
{
    /// <summary>
    /// Represents a list that is serialized into XML with a count attribute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>Based on http://docs.telerik.com/help/openaccess-classic/openaccess-tasks-using-xmlserializer-with-generic-lists-and-persistent-objects.html </remarks>
    public class XmlCountList<T> : List<T>, IXmlSerializable
    {
        public XmlCountList()
        {
        }

        public XmlCountList(int capacity) : base(capacity)
        {
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();

            XmlSerializer inner = new XmlSerializer(typeof(T), "");

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                T e = (T)inner.Deserialize(reader);
                Add(e);
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("count", Count.ToString(NumberFormatInfo.InvariantInfo));

            if (Count == 0)
                return;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer inner = new XmlSerializer(typeof(T), "");
            for (int i = 0; i < Count; i++)
            {
                inner.Serialize(writer, this[i], ns);
            }
        }

        XmlSchema IXmlSerializable.GetSchema() => null;
    }
}
