using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlUtils
{
    /// <summary>
    /// Represents a list that is serialized into XML with a count attribute.
    /// For types that implement IXmlSerializable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlCountListEx<T> : List<T>, IXmlSerializable
        where T : IXmlSerializable, new()
    {
        private readonly string _elementName;

        public XmlCountListEx(string elementName)
        {
            _elementName = elementName;
        }

        public XmlCountListEx(string elementName, int capacity) : base(capacity)
        {
            _elementName = elementName;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            string countAttr = reader.GetAttribute("count");
            if (countAttr != null)
            {
                int count = int.Parse(countAttr, NumberFormatInfo.InvariantInfo);
                if (count > 0)
                    Capacity = count;
            }

            reader.ReadStartElement();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                T element = new T();
                element.ReadXml(reader);
                Add(element);
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("count", Count.ToString(NumberFormatInfo.InvariantInfo));

            if (Count == 0)
                return;

            for (int i = 0; i < Count; i++)
            {
                writer.WriteStartElement(_elementName);
                this[i].WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        XmlSchema IXmlSerializable.GetSchema() => null;
    }
}
