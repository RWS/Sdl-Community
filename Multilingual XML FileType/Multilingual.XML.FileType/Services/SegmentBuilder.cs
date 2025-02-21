using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services.Entities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Services
{
	public class SegmentBuilder
	{
		public const string EmbeddedContentMetadataKey = "OriginalEmbeddedContent";
		public const string EmbeddedContentMetadataEndKey = "__end_OriginalEmbeddedContent";

		private readonly EntityService _entityService;

		public SegmentBuilder(EntityService entityService)
		{
			ItemFactory = entityService.FrameworkService.DocumentItemFactory;
			PropertiesFactory = entityService.FrameworkService.PropertiesFactory;

			_entityService = entityService;
		}

		public IDocumentItemFactory ItemFactory { get; }

		public IPropertiesFactory PropertiesFactory { get; }

		public ITranslationOrigin CreateTranslationOrigin()
		{
			return ItemFactory.CreateTranslationOrigin();
		}

		public IParagraphUnit CreateStructureParagraphUnit()
		{
			var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Structure);
			var contextProperties = ItemFactory.PropertiesFactory.CreateContextProperties();
			var contextInfo = CreateContextInfo("StructureParagraphUnit");

			contextProperties.Contexts.Add(contextInfo);
			paragraphUnit.Properties.Contexts = contextProperties;

			return paragraphUnit;
		}

		public IContextInfo CreateContextInfo(string contextType)
		{
			return ItemFactory.PropertiesFactory.CreateContextInfo(contextType);
		}

		public IContextInfo CreateMultilingualParagraphContextInfo()
		{
			var multilingualParagraphContextInfo =
				ItemFactory.PropertiesFactory.CreateContextInfo(Constants.MultilingualParagraphUnit);
			multilingualParagraphContextInfo.Purpose = ContextPurpose.Information;
			multilingualParagraphContextInfo.DisplayCode = "MP";
			multilingualParagraphContextInfo.ContextType = Constants.MultilingualParagraphUnit;
			multilingualParagraphContextInfo.DisplayName = "Multilingual Paragraph";
			multilingualParagraphContextInfo.DisplayColor = Color.DarkSeaGreen;
			multilingualParagraphContextInfo.Description = "A paragraph of text";

			return multilingualParagraphContextInfo;
		}

		public IContextInfo CreateElementPropertyParagraphContextInfo(string name, string value)
		{
			var multilingualParagraphContextInfo = ItemFactory.PropertiesFactory.CreateContextInfo(Constants.ElementPropertyParagraphUnit);
			multilingualParagraphContextInfo.Purpose = ContextPurpose.Information;
			multilingualParagraphContextInfo.DisplayCode = "EP";
			multilingualParagraphContextInfo.ContextType = Constants.ElementPropertyParagraphUnit;
			multilingualParagraphContextInfo.DisplayName = name;
			multilingualParagraphContextInfo.DisplayColor = Color.LightYellow;
			multilingualParagraphContextInfo.Description = value;

			return multilingualParagraphContextInfo;
		}

		public IParagraphUnit CreateParagraphUnit(IParagraphUnitProperties properties)
		{
			var paragraphUnit = ItemFactory.CreateParagraphUnit(properties?.LockType ?? LockTypeFlags.Unlocked);
			if (properties != null)
			{
				paragraphUnit.Properties = properties.Clone() as IParagraphUnitProperties;
			}

			return paragraphUnit;
		}

		public ISegmentPair CreateSegmentPair(ISegment source, ISegment target)
		{
			return ItemFactory.CreateSegmentPair(source, target);
		}

		public ISegment CreateSegment(ISegmentPairProperties segmentPairProperties)
		{
			return ItemFactory.CreateSegment(segmentPairProperties);
		}

		public IContextInfo CreateContextInfo(ParagraphUnitContext context)
		{
			var contextInfo = PropertiesFactory.CreateContextInfo(context.ContextType);
			contextInfo.DisplayName = context.DisplayName;
			contextInfo.DisplayCode = context.DisplayCode;
			contextInfo.Description = context.Description;
			foreach (var metaData in context.MetaData)
			{
				contextInfo.SetMetaData(metaData.Key, metaData.Value);
			}

			return contextInfo;
		}

		public ISegment CreateSegment(string text, ISegmentPairProperties pair)
		{
			var segment = ItemFactory.CreateSegment(pair);
			if (text == null)
			{
				return segment;
			}

			var fragments = _entityService.ConvertKnownEntitiesInMarkupData(text, EntityRule.Parser);
			foreach (var fragment in fragments)
			{
				segment.Add(fragment);
			}

			return segment;
		}

		public ISegment CreateSegment(XmlNode xmlNode, ISegmentPairProperties pair, SegmentationHint segmentationHint)
		{
			var segment = ItemFactory.CreateSegment(pair);
			if (xmlNode == null)
			{
				return segment;
			}

			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				switch (childNode.NodeType)
				{
					case XmlNodeType.Text:
						{
							var fragments = _entityService.ConvertKnownEntitiesInMarkupData(childNode.InnerText, EntityRule.Parser);
							foreach (var fragment in fragments)
							{
								segment.Add(fragment);
							}
						}
						break;
					case XmlNodeType.Element when IsPlaceholderTag(childNode):
						segment.Add(CreatePlaceholderTag(childNode, segmentationHint));
						break;
					case XmlNodeType.Element:
						segment.Add(CreateTagPair(childNode, segmentationHint));
						break;
					case XmlNodeType.CDATA:
						segment.Add(CreatePlaceholderTag(childNode, segmentationHint));
						break;
					case XmlNodeType.Entity:
						{
							// TODO process entity
							var fragments = _entityService.ConvertKnownEntitiesInMarkupData(childNode.InnerXml, EntityRule.Parser);
							foreach (var fragment in fragments)
							{
								segment.Add(fragment);
							}
						}
						break;
					case XmlNodeType.EntityReference:
						// TODO add to context
						break;
				}
			}

			return segment;
		}

		public static IEnumerable<XNode> ParseXmlFragment(string xml)
		{
			var settings = new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Fragment,
				IgnoreWhitespace = true
			};

			using (var stringReader = new StringReader(xml))
			using (var xmlReader = XmlReader.Create(stringReader, settings))
			{
				xmlReader.MoveToContent();
				while (xmlReader.ReadState != ReadState.EndOfFile)
				{
					yield return XNode.ReadFrom(xmlReader);
				}
			}
		}

		public IFormattingGroup GetFormattingGroup(XmlNode xmlNode)
		{
			var formatting = PropertiesFactory.FormattingItemFactory.CreateFormatting();

			switch (xmlNode.Name.ToLower())
			{
				case "b":
				case "strong":
					formatting.Add(new Bold(true));
					break;
				case "i":
					formatting.Add(new Italic(true));
					break;
				case "u":
				case "ins":
					formatting.Add(new Underline(true));
					break;
				case "s":
				case "del":
					formatting.Add(new Strikethrough(true));
					break;
					//case "font":
					//	var color = ColorTranslator.FromHtml(tag.ColorAttributeValue);
					//	formatting.Add(new TextColor(color));
					//	break;
			}

			return formatting;
		}

		public bool IsPlaceholderTag(XmlNode xmlNode)
		{
			return xmlNode.NodeType == XmlNodeType.Element &&
				   xmlNode.ChildNodes.Count == 0 &&
				   xmlNode.OuterXml.Replace(" ", string.Empty).EndsWith("/>");
		}

		public IPlaceholderTag CreatePlaceholderTag(XmlNode xmlNode, SegmentationHint segmentationHint)
		{
			var properties = PropertiesFactory.CreatePlaceholderTagProperties(xmlNode.OuterXml);
			properties.DisplayText = xmlNode.Name;
			properties.CanHide = false;
			properties.SegmentationHint = segmentationHint;
			return ItemFactory.CreatePlaceholderTag(properties);
		}

		public IPlaceholderTag CreatePlaceholderTag(string content, bool canHide, SegmentationHint segmentationHint)
		{
			var properties = PropertiesFactory.CreatePlaceholderTagProperties(content);
			properties.DisplayText = content;
			properties.CanHide = canHide;
			properties.SegmentationHint = segmentationHint;
			return ItemFactory.CreatePlaceholderTag(properties);
		}

		private ITagPair CreateTagPair(XmlNode xmlNode, SegmentationHint segmentationHint)
		{
			var attributes = GetAttributesString(xmlNode);
			var tagContent = "<" + xmlNode.Name + attributes + ">";

			var startTagProperties = PropertiesFactory.CreateStartTagProperties(tagContent);

			var formatting = GetFormattingGroup(xmlNode);
			startTagProperties.Formatting = formatting;
			startTagProperties.DisplayText = xmlNode.Name;
			startTagProperties.CanHide = false;

			var endTagProperties = PropertiesFactory.CreateEndTagProperties("</" + xmlNode.Name + ">");
			endTagProperties.DisplayText = xmlNode.Name;
			endTagProperties.CanHide = false;


			var tagPair = GetTagPair(xmlNode, segmentationHint, startTagProperties, endTagProperties);

			return tagPair;
		}

		private ITagPair GetTagPair(XmlNode xmlNode, SegmentationHint segmentationHint, IStartTagProperties startTagProperties, IEndTagProperties endTagProperties)
		{
			var tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);

			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				switch (childNode.NodeType)
				{
					case XmlNodeType.Text:
						{
							var fragments = _entityService.ConvertKnownEntitiesInMarkupData(childNode.InnerText, EntityRule.Parser);
							foreach (var fragment in fragments)
							{
								tagPair.Add(fragment);
							}
						}
						break;
					case XmlNodeType.Element when IsPlaceholderTag(childNode):
						tagPair.Add(CreatePlaceholderTag(childNode, segmentationHint));
						break;
					case XmlNodeType.Element:
						tagPair.Add(CreateTagPair(childNode, segmentationHint));
						break;
					case XmlNodeType.CDATA:
						tagPair.Add(CreatePlaceholderTag(childNode, segmentationHint));
						break;
					case XmlNodeType.Entity:
						{
							// TODO process entity
							var fragments = _entityService.ConvertKnownEntitiesInMarkupData(childNode.InnerXml, EntityRule.Parser);
							foreach (var fragment in fragments)
							{
								tagPair.Add(fragment);
							}
						}
						break;
					case XmlNodeType.EntityReference:
						// TODO add to context
						break;
				}
			}

			return tagPair;
		}

		private static string GetAttributesString(XmlNode xmlNode)
		{
			var attributes = string.Empty;
			if (xmlNode.Attributes?.Count > 0)
			{
				attributes = xmlNode.Attributes.Cast<XmlAttribute>().Aggregate(attributes, (current, itemAttribute)
					=> current + " " + itemAttribute.Name + "=\"" + itemAttribute.Value + "\"");
			}

			return attributes;
		}

		public IAbstractMarkupData CreatePlaceholder(string tagId, string tagContent, ref List<string> existingTagIds)
		{
			// Dev Notes: the tagContent is switched with the Display text to align with how the tags are 
			// recreated by the XLIFF 1.2 parser from the framework

			var textProperties = PropertiesFactory.CreatePlaceholderTagProperties("<ph id=\"" + tagId + "\"/>");
			textProperties.DisplayText = tagContent;
			textProperties.CanHide = false;
			textProperties.SetMetaData("localName", "ph");
			textProperties.SetMetaData("displayText", tagContent);
			textProperties.SetMetaData("attribute:id", tagId);

			if (existingTagIds.Contains(textProperties.TagId.Id))
			{
				textProperties.TagId = !existingTagIds.Contains(tagId)
					? new TagId(tagId)
					: new TagId(GetUniqueTagPairId(existingTagIds));
			}

			if (!existingTagIds.Contains(textProperties.TagId.Id))
			{
				existingTagIds.Add(textProperties.TagId.Id);
			}

			return ItemFactory.CreatePlaceholderTag(textProperties);
		}

		public IAbstractMarkupData Text(string text)
		{
			var textProperties = PropertiesFactory.CreateTextProperties(text);
			return ItemFactory.CreateText(textProperties);
		}

		public ISegmentPairProperties CreateSegmentPairProperties()
		{
			var properties = ItemFactory.CreateSegmentPairProperties();
			properties.TranslationOrigin = CreateTranslationOrigin();
			return properties;
		}

		public IComment CreateComment(string text, string author, Severity severity, DateTime dateTime, string version)
		{
			var comment = PropertiesFactory.CreateComment(text, author, severity);
			if (dateTime != DateTime.MinValue && dateTime != DateTime.MaxValue)
			{
				comment.Date = dateTime;
			}

			if (string.IsNullOrEmpty(version))
			{
				comment.Version = version;
			}

			return comment;
		}

		public IAbstractMarkupData CreateCommentContainer(string text, string author, Severity severity, DateTime dateTime, string version)
		{
			var comment = CreateComment(text, author, severity, dateTime, version);

			var commentProperties = PropertiesFactory.CreateCommentProperties();
			commentProperties.Add(comment);
			var commentMarker = ItemFactory.CreateCommentMarker(commentProperties);

			return commentMarker;
		}

		public ITagPair CreateTagPair(IStartTagProperties startTagProperties, IEndTagProperties endTagProperties)
		{
			var tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);
			return tagPair;
		}

		public ITagPair CreateTagPair(string tagId, string tagContent, ref List<string> existingTagIds)
		{
			var tagName = GetStartTagName(tagContent, out var refId);
			var startTagProperties = PropertiesFactory.CreateStartTagProperties("<" + tagName + " id=\"" + tagId + "\">");
			startTagProperties.DisplayText = tagContent;
			startTagProperties.CanHide = false;
			startTagProperties.SetMetaData("localName", tagName);
			startTagProperties.SetMetaData("displayText", tagContent);
			startTagProperties.SetMetaData("attribute:id", tagId);

			if (existingTagIds.Contains(startTagProperties.TagId.Id))
			{
				startTagProperties.TagId = !existingTagIds.Contains(tagId)
					? new TagId(tagId)
					: new TagId(GetUniqueTagPairId(existingTagIds));
			}

			if (!existingTagIds.Contains(startTagProperties.TagId.Id))
			{
				existingTagIds.Add(startTagProperties.TagId.Id);
			}

			var endTagProperties = PropertiesFactory.CreateEndTagProperties("</" + tagName + ">");
			endTagProperties.DisplayText = "</" + tagName + ">";
			endTagProperties.SetMetaData("localName", tagName);
			endTagProperties.SetMetaData("displayText", "</" + tagName + ">");
			endTagProperties.SetMetaData("attribute:id", tagId);


			//TODO formatting example
			//var xItem = _formattingFactory.CreateFormattingItem("italic", "True");
			//x.Formatting = _formattingFactory.CreateFormatting();
			//x.Formatting.Add(xItem);

			var tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);


			return tagPair;
		}

		private string GetUniqueTagPairId(List<string> existingTagIds)
		{
			var id = 1;
			while (existingTagIds.Contains(id.ToString()))
			{
				id++;
			}

			return id.ToString();
		}

		public IAbstractMarkupData CreateLockedContent()
		{
			var lockedContentProperties = PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);
			var lockedContent = ItemFactory.CreateLockedContent(lockedContentProperties);
			return lockedContent;
		}

		public string GetStartTagName(string text, out string refId)
		{
			var tagName = string.Empty;
			refId = string.Empty;
			var regexTagName = new Regex(@"\<(?<name>[^\s""\>]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			var regexAttribute = new Regex(@"\s+(?<name>[^\s""]+)\=""(?<value>[^""]*)""", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			var m = regexTagName.Match(text);
			if (m.Success)
			{
				tagName = m.Groups["name"].Value;
			}

			var mc = regexAttribute.Matches(text);
			if (mc.Count > 0)
			{
				foreach (Match match in mc)
				{
					var attValue = match.Groups["value"].Value;
					var attName = match.Groups["name"].Value;

					if (string.Compare(attName, "id", StringComparison.OrdinalIgnoreCase) == 0)
					{
						refId = attValue;
					}
				}
			}

			return tagName;
		}

		public string GetEndTagName(string text)
		{
			var tagName = string.Empty;

			var regexTagName = new Regex(@"\<\s*\/\s*(?<name>[^\s\>]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			var m = regexTagName.Match(text);
			if (m.Success)
			{
				tagName = m.Groups["name"].Value;
			}

			return tagName;
		}

		public string GetEmptyTagName(string text)
		{
			var tagName = string.Empty;

			var regexTagName = new Regex(@"\<\s*(?<name>[^\s\>\/]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			var m = regexTagName.Match(text);
			if (m.Success)
			{
				tagName = m.Groups["name"].Value;
			}

			return tagName;
		}
	}
}
