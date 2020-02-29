using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace XmlUtils
{
    /// <summary>
    /// Static methods for using loading and writing configuration files using reflection.
    /// </summary>
    public static class ReflectionConfig
    {
        private const string AttributeName = "value";

        /// <summary>
        /// Serializes an object into an xml file.
        /// The values of object's properties are stored in XML attributes.
        /// </summary>
        /// <param name="filename">The target filename.</param>
        /// <param name="obj">The object to be serialized.</param>
        public static void SaveToXml(string filename, object obj)
        {
            using XmlWriter writer = XmlWriter.Create(filename, new XmlWriterSettings { Indent = true });
            var objType = obj.GetType();

            writer.WriteStartDocument();
            writer.WriteStartElement(objType.Name);

            foreach (var prop in objType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanWrite) // Skip read only properties
                    continue;

                writer.WriteStartElement(prop.Name);

                object value = prop.GetValue(obj);

                writer.WriteAttributeString(
                    AttributeName,
                    Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture).ToString());

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Deserializes an object from an XML file.
        /// </summary>
        /// <param name="filename">The file to read from.</param>
        /// <param name="obj">The target object.</param>
        public static void LoadFromXml(string filename, object obj)
        {
            var cfgType = obj.GetType();

            using XmlReader reader = XmlReader.Create(filename);
            reader.MoveToContent();

            while (reader.Read() && reader.IsStartElement())
            {
                var prop = cfgType.GetProperty(reader.LocalName);
                if (prop is null)
                    continue;

                string value = reader.GetAttribute(AttributeName);
                if (value is null)
                    continue;

                if (prop.PropertyType.IsEnum)
                    prop.SetValue(obj, Enum.Parse(prop.PropertyType, value));
                else
                    prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType, CultureInfo.InvariantCulture));
            }
        }
    }
}
