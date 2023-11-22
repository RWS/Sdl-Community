using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using InterpretBank.GlossaryExchangeService.Wrappers.Interface;

namespace InterpretBank.GlossaryExchangeService.Wrappers
{
	public class TbxDocumentWrapper : IXmlReaderWriterWrapper
	{
		public XmlWriter CreateTbx(string path, string glossaryName, string subGlossaryName)
		{
			var xmlWriter = XmlWriter.Create(path, new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Auto });

			xmlWriter.WriteStartDocument();
			xmlWriter.WriteDocType("martif", "SYSTEM", "TBXcoreStructV02.dtd", null);

			xmlWriter.WriteStartElement("martif");
			xmlWriter.WriteAttributeString("lang", XNamespace.Xml.ToString(), "en");
			xmlWriter.WriteAttributeString("type", null, "TBX-Default");
			xmlWriter.WriteStartElement("text");
			xmlWriter.WriteStartElement("body");

			xmlWriter.WriteStartElement("glossary");
			xmlWriter.WriteValue(glossaryName);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteStartElement("subglossary");
			if (subGlossaryName != null) xmlWriter.WriteValue(subGlossaryName);
			xmlWriter.WriteEndElement();
			return xmlWriter;
		}

		public IEnumerable<XElement> GetTermElements(string path)
		{
			using var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse });

			while (reader.Read())
			{
				if (reader.Name == "termEntry")
				{
					yield return (XElement)XNode.ReadFrom(reader);
				}
			}
		}
	}
}