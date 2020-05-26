using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace XmlUtils
{
    /// <summary>
    /// Provides helper methods for working with XML files.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Serializes an object into an XML file.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="filename">Target filename.</param>
        /// <param name="obj">Object to serialize</param>
        /// <param name="nsPrefix">XML namespace prefix.</param>
        /// <param name="ns">XML namespace.</param>
        public static void Serialize<T>(string filename, T obj, string nsPrefix = "", string ns = "")
        {
            var nameSpace = new XmlSerializerNamespaces();
            nameSpace.Add(nsPrefix, ns);

            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
            };

            using XmlWriter writer = XmlWriter.Create(filename, settings);
            serializer.Serialize(writer, obj, nameSpace);
        }

        /// <summary>
        /// Deserializes an object from an XML file.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="filename">Filename from which object will be deserialized.</param>
        /// <param name="ns">XML namespace.</param>
        /// <returns>Deserialized object.</returns>
        public static T Deserialize<T>(string filename, string ns = "")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), ns);
            using StreamReader file = new StreamReader(filename);
            return (T)serializer.Deserialize(file);
        }

        /// <summary>
        /// Returns true if the root element name of the XML file matches the parameter.
        /// </summary>
        /// <param name="filename">Name of the file to check.</param>
        /// <param name="rootElement">Name of the root element to match.</param>
        /// <returns></returns>
        public static async Task<bool> ValidateRootElementAsync(string filename, string rootElement)
        {
            using XmlReader reader = XmlReader.Create(filename, new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync().ConfigureAwait(false);
            return reader.LocalName == rootElement;
        }

        /// <summary>
        /// Returns true if the root element name of the XML file matches the parameter.
        /// </summary>
        /// <param name="filename">Name of the file to check.</param>
        /// <param name="rootElement">Name of the root element to match.</param>
        /// <returns></returns>
        public static bool ValidateRootElement(string filename, string rootElement)
        {
            using XmlReader reader = XmlReader.Create(filename);
            reader.MoveToContent();
            return reader.LocalName == rootElement;
        }

        /// <summary>
        /// Returns the root element name of the XML file.
        /// </summary>
        /// <param name="filename">Name of the file to check.</param>
        /// <returns>The root element name of the file.</returns>
        public static string GetRootElementName(string filename)
        {
            using XmlReader reader = XmlReader.Create(filename);
            reader.MoveToContent();
            return reader.LocalName;
        }
    }
}
