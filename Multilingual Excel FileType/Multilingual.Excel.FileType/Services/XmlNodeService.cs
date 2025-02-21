using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Attribute = Multilingual.Excel.FileType.Models.Attribute;

namespace Multilingual.Excel.FileType.Services
{
	public class XmlNodeService
	{
		private readonly XmlDocument _document;
		private readonly XmlNamespaceManager _nsmgr;

		public XmlNodeService(XmlDocument document, XmlNamespaceManager namespaceManager)
		{
			_document = document;
			_nsmgr = namespaceManager;
		}

		public XmlNode AddXmlNode(XmlNode xmlUnit, string xPath)
		{
			var regexAttributesBlock = new Regex(@"\[(?<block>[^\]]*)\]", RegexOptions.None);

			var xmlNode = xmlUnit;
			var pathParts = GetPathParts(xPath.Trim('/'));
			foreach (var pathPart in pathParts)
			{
				var xmlNodeName = pathPart.Trim('/');
				if (string.IsNullOrEmpty(xmlNodeName))
				{
					continue;
				}

				var validAttributes = new List<Models.Attribute>();
				var matchAttributesBlock = regexAttributesBlock.Match(xmlNodeName);
				var normalizedXmlNodeName = xmlNodeName;
				if (matchAttributesBlock.Success)
				{
					normalizedXmlNodeName = regexAttributesBlock.Replace(xmlNodeName, string.Empty);
					var attributesBlock = matchAttributesBlock.Groups["block"].Value;

					var xPathAttributes = GetEqualAttributes(attributesBlock);
					var xPathOperators = GetOperators(attributesBlock);

					if (!xPathOperators.Contains("or"))
					{
						validAttributes = xPathAttributes;
					}
				}

				var queryNode = GetQueryNode(xmlNodeName, validAttributes);

				var existingNode = xmlNode.SelectSingleNode(queryNode, _nsmgr);
				if (existingNode == null && !string.IsNullOrEmpty(normalizedXmlNodeName))
				{
					xmlNodeName = normalizedXmlNodeName;
					queryNode = GetQueryNode(normalizedXmlNodeName, validAttributes);
					existingNode = xmlNode.SelectSingleNode(queryNode, _nsmgr);
				}
				if (existingNode == null)
				{
					// check that the namespace is inherited from the parent and not the default
					if (xmlNodeName.Contains(":"))
					{
						var xmlUnitNsPrefix = string.IsNullOrEmpty(xmlUnit.Prefix) ? null : xmlUnit.Prefix;

						var nsName = xmlNodeName.Substring(0, xmlNodeName.IndexOf(":", StringComparison.Ordinal));
						var nsValue = xmlNodeName.Substring(xmlNodeName.IndexOf(":", StringComparison.Ordinal) + 1);

						if (string.Compare(nsName, xmlUnitNsPrefix, StringComparison.InvariantCultureIgnoreCase) != 0)
						{
							xmlNodeName = xmlUnitNsPrefix + (!string.IsNullOrEmpty(xmlUnitNsPrefix) ? ":" : string.Empty) + nsValue;
						}
					}

					var xmlUnitNsUri = string.IsNullOrEmpty(xmlUnit.NamespaceURI.Trim()) ? null : xmlUnit.NamespaceURI;
					var newXmlNode = _document.CreateNode(XmlNodeType.Element, xmlNodeName, xmlUnitNsUri);
					if (validAttributes.Count > 0)
					{
						foreach (var attribute in validAttributes)
						{
							var xmlAttribute = _document.CreateAttribute(attribute.Name);
							xmlAttribute.Value = attribute.Value;
							newXmlNode.Attributes?.Append(xmlAttribute);
						}
					}

					xmlNode = xmlNode.AppendChild(newXmlNode);
				}
				else
				{
					xmlNode = existingNode;
				}
			}

			return xmlNode;
		}

		private static string GetQueryNode(string xmlNodeName, List<Attribute> validAttributes)
		{
			var queryNode = xmlNodeName;
			if (validAttributes.Count > 0)
			{
				var attributesQuery = string.Empty;
				foreach (var validAttribute in validAttributes)
				{
					attributesQuery += (string.IsNullOrEmpty(attributesQuery) ? string.Empty : " and ")
									   + "@" + validAttribute.Name + "='" + validAttribute.Value + "'";
				}

				queryNode = queryNode + "[" + attributesQuery + "]";
			}

			return queryNode;
		}

		private static List<string> GetOperators(string attributesBlock)
		{
			var foundOperators = new List<string>();
			var attributesBlockToLower = attributesBlock.ToLower();
			var operators = new List<string>
				{ " | ", " + ", " - ", " * ", " div ", "=", "!=", "<", "<=", ">", ">=", " or ", " and ", " mod " };
			foreach (var @operator in operators)
			{
				if (attributesBlockToLower.Contains(@operator))
				{
					if (!foundOperators.Contains(@operator.Trim()))
					{
						foundOperators.Add(@operator.Trim());
					}
				}
			}

			return foundOperators;
		}

		private static List<Attribute> GetEqualAttributes(string attributesBlock)
		{
			var equalAttributes = new List<Models.Attribute>();
			var regexAttribute = new Regex(@"\@(?<name>[^\>\|\<\=\+\-\*\!\(\)\s]+)\s*(?<!\<)\=(?!\>)\s*(?<value>[^\s]+)", RegexOptions.None);

			var attributes = regexAttribute.Matches(attributesBlock);
			if (attributes.Count > 0)
			{
				foreach (Match attribute in attributes)
				{
					var name = attribute.Groups["name"].Value;
					var value = attribute.Groups["value"].Value;
					value = value.Replace("'", "");
					value = value.Replace("\"", "");

					equalAttributes.Add(new Models.Attribute
					{
						Name = name,
						Value = value,
						Operator = "="
					});
				}
			}

			return equalAttributes;
		}

		private static IEnumerable<string> GetPathParts(string xPath)
		{
			var charIndex = 0;
			var previousChar = string.Empty;
			var currentPart = string.Empty;
			var parts = new List<string>();
			while (charIndex < xPath.Length)
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
	}
}
