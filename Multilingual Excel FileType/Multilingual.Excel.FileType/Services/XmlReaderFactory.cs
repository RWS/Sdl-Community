using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Multilingual.Excel.FileType.Models;

namespace Multilingual.Excel.FileType.Services
{
    public class XmlReaderFactory
    {
	    public XmlReader CreateReader(Stream inputStream, Encoding encoding, bool disableUndeclaredEntityCheck)
        {
            var xmlReaderSettings = CreateXmlReaderSettings();

            var streamReader = new StreamReader(inputStream, encoding);
            var xmlReader = XmlReader.Create(streamReader, xmlReaderSettings);

            UpdateReaderProperties(xmlReader, disableUndeclaredEntityCheck);
            return xmlReader;
        }

	    public XmlReader CreateReader(string path, bool disableUndeclaredEntityCheck)
	    {
		    var xmlReaderSettings = CreateXmlReaderSettings();
		    var xmlReader = XmlReader.Create(path, xmlReaderSettings);

		    UpdateReaderProperties(xmlReader, disableUndeclaredEntityCheck);
		    return xmlReader;
	    }

        private void UpdateReaderProperties(XmlReader xmlReader, bool disableUndeclaredEntityCheck)
        {
            try
            {
                var type = xmlReader.GetType();
                UpdateProperty("Normalization", false, type, xmlReader);
                UpdateProperty("DisableUndeclaredEntityCheck", disableUndeclaredEntityCheck, type, xmlReader);
            }
            catch
            {
                // empty
            }
        }

        private void UpdateProperty(string propName, bool value, Type type, XmlReader xmlreaderInstance)
        {
            var prop = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop == null) return;
            prop.SetValue(xmlreaderInstance, value);
        }

        private XmlReaderSettings CreateXmlReaderSettings()
        {
            var xmlReaderSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                ConformanceLevel = ConformanceLevel.Auto,
                XmlResolver = null,
                CheckCharacters = false,
            };
            return xmlReaderSettings;
        }

        public XmlReader CreateReader(StringReader inputStream, IEnumerable<XmlNamespace> xmlNamespaces)
        {
            var xmlReaderSettings = CreateXmlReaderSettings();
            var context = SetupReaderContext(xmlNamespaces);

            var xmlReader = XmlReader.Create(inputStream, xmlReaderSettings, context);
            UpdateReaderProperties(xmlReader, false);

            return xmlReader;
        }

        private XmlParserContext SetupReaderContext(IEnumerable<XmlNamespace> xmlNamespaces)
        {
            var nt = new NameTable();
            var nsmgr = new XmlNamespaceManager(nt);
            foreach (var namespaceDef in xmlNamespaces)
            {
                nsmgr.AddNamespace(namespaceDef.Prefix, namespaceDef.Uri);
            }

            return new XmlParserContext(null, nsmgr, null, XmlSpace.None);
        }

        public XmlReader CreateReader(Stream inputStream, Encoding encoding, XmlReaderSettings settings)
        {
            var streamReader = new StreamReader(inputStream, encoding);
            var xmlReader = XmlReader.Create(streamReader, settings);
            UpdateReaderProperties(xmlReader, true);

            return xmlReader;
        }

    }
}
