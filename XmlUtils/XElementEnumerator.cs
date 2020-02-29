using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XmlUtils
{
    /// <summary>
    /// A class that enumerates XElements from an XmlReader.
    /// </summary>
    public sealed class XElementEnumerator : IEnumerable<XElement>, IDisposable
    {
        private readonly XmlReader _reader;
        private readonly string _elementName;

        public bool IncludeEmptyElements { get; set; } = true;

        public XElementEnumerator(XmlReader reader, string elementName)
        {
            _reader = reader;
            _elementName = elementName;
        }

        public XElementEnumerator(string filename, string elementName)
        {
            _reader = XmlReader.Create(filename);
            _elementName = elementName;
        }

        public void Dispose() => _reader.Dispose();

        public IEnumerator<XElement> GetEnumerator()
        {
            while (_reader.ReadToFollowing(_elementName))
            {
                if (!_reader.IsEmptyElement || IncludeEmptyElements)
                {
                    var subTree = _reader.ReadSubtree();
                    var xel = XElement.Load(subTree);
                    subTree.Dispose();
                    yield return xel;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
