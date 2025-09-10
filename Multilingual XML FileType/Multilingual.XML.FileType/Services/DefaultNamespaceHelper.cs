using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Multilingual.XML.FileType.Services
{
	public class DefaultNamespaceHelper
	{
		private readonly XmlReaderFactory _xmlReaderFactory;

		public DefaultNamespaceHelper(XmlReaderFactory xmlReaderFactory)
		{
			_xmlReaderFactory = xmlReaderFactory;
		}

        public void AddAllNamespacesFromDocument(XmlDocument doc, XmlNamespaceManager nsmgr)
        {
            var root = doc.DocumentElement;
            if (root == null) return;

            foreach (XmlAttribute attr in root.Attributes)
            {
                if (attr.Prefix == "xmlns")
                {
                    nsmgr.AddNamespace(attr.LocalName, attr.Value);
                }
                else if (attr.Name == "xmlns")
                {
                    nsmgr.AddNamespace(string.Empty, attr.Value); // default namespace
                }
            }
        }

        public string GetXmlNameSpaceUri(string filePath, Encoding encoding)
		{
			string namespaceUri;
			using (var fs = new FileStream(filePath, FileMode.Open))
			{
				namespaceUri = GetDefaultNamespaceUri(fs, encoding);

				fs.Flush();
				fs.Close();
			}

			return namespaceUri;
		}

		public List<XmlNameSpace> GetXmlNameSpaces(Stream fileStream)
		{
			var document = new XmlDocument();
			document.Load(fileStream);

			return XmlNameSpaces(document);
		}

		public List<XmlNameSpace> GetXmlNameSpaces(XmlDocument document)
		{
			return XmlNameSpaces(document);
		}

		public List<XmlNameSpace> GetXmlNameSpaces(string filePath)
		{
			var document = new XmlDocument();
			document.Load(filePath);

			return XmlNameSpaces(document);
		}

		public void AddXmlNameSpacesFromDocument(XmlNamespaceManager nsmgr, XmlDocument document, XmlNameSpace defaultNamespace)
		{
			var xmlNameSpaces = GetXmlNameSpaces(document);
			foreach (var xmlNameSpace in xmlNameSpaces)
			{
				AddNameSpace(nsmgr, xmlNameSpace);
			}

			AddNameSpace(nsmgr, defaultNamespace);
		}

		private static void AddNameSpace(XmlNamespaceManager nsmgr, XmlNameSpace xmlNameSpace)
		{
			if (!string.IsNullOrEmpty(xmlNameSpace.Name)
			    && !string.IsNullOrEmpty(xmlNameSpace.Value)
			    && string.Compare(xmlNameSpace.Name, "xmlns", StringComparison.InvariantCultureIgnoreCase) != 0
			    && !nsmgr.HasNamespace(xmlNameSpace.Name))
			{
				nsmgr.AddNamespace(xmlNameSpace.Name, xmlNameSpace.Value);
			}
		}

		private static List<XmlNameSpace> XmlNameSpaces(XmlDocument document)
		{
			var xmlNameSpaces = new List<XmlNameSpace>();

			if (document.DocumentElement != null)
			{
				foreach (XmlAttribute xmlAttribute in document.DocumentElement.Attributes)
				{
					if (string.IsNullOrEmpty(xmlAttribute.NamespaceURI))
					{
						continue;
					}

					var xmlNameSpace = new XmlNameSpace
					{
						Name = xmlAttribute.LocalName,
						Value = xmlAttribute.Value
					};

					xmlNameSpaces.Add(xmlNameSpace);
				}
			}

			return xmlNameSpaces;
		}

		public string UpdateXPathWithNamespace(string xPath, string ns)
		{
			var parts = GetNamespaceParts(xPath);

			var hasNamespace = parts.Any(a => a.Contains(":"));

			if (hasNamespace)
			{
				return xPath;
			}

			var updatedNs = string.Empty;
			foreach (var part in parts)
			{
				if (!part.StartsWith("/"))
				{
					updatedNs += ns + ":" + part;
				}
				else
				{
					updatedNs += part;
				}
			}

			return updatedNs;
		}

		private static List<string> GetNamespaceParts(string xPath)
		{
			var charIndex = 0;
			var previousChar = string.Empty;
			var currentPart = string.Empty;
			var parts = new List<string>();
			while (charIndex < xPath?.Length)
			{
				var character = xPath[charIndex].ToString();

				if (charIndex == 0)
				{
					currentPart = character;
				}
				else if (character != "/" && previousChar == "/")
				{
					parts.Add(currentPart);
					currentPart = character;
				}
				else
				{
					currentPart += character;
				}

				previousChar = character;
				charIndex++;
			}

			if (!string.IsNullOrEmpty(currentPart))
			{
				parts.Add(currentPart);
			}

			return parts;
		}

		private string GetDefaultNamespaceUri(Stream inputStream, Encoding encoding)
		{
			var reader = _xmlReaderFactory.CreateReader(inputStream, encoding, true);
			var ns = GetDefaultNamespaceUri(reader);
			inputStream.Seek(0, SeekOrigin.Begin);
			return ns;
		}

		private string GetDefaultNamespaceUri(XmlReader reader)
		{
			var defaultNamespaceValue = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
				
					if (reader.MoveToAttribute("xmlns"))
					{
						defaultNamespaceValue = reader.Value;
					}

					else if (!string.IsNullOrEmpty(reader.Prefix))
					{
						var defaultNamespaceName = reader.Prefix;
						if (reader.HasAttributes)
						{
							while (reader.MoveToNextAttribute())
							{
								if (defaultNamespaceName == reader.LocalName)
								{
									defaultNamespaceValue = reader.Value;
									break;
								}
							}

							// Move the reader back to the element node.
							reader.MoveToElement();
						}
					}

					break;
				}
			}

			return defaultNamespaceValue;
		}
	}
}
