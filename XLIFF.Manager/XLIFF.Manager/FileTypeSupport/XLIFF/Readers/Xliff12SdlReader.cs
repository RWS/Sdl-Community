using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers
{
	public class Xliff12SdlReader : IXliffReader
	{
		private const string NsPrefix = "sdlxliff";
		private readonly SegmentBuilder _segmentBuilder;

		public Xliff12SdlReader(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}

		public Xliff ReadXliff(string filePath)
		{
			var xliff = new Xliff();

			var xmlTextReader = new XmlTextReader(filePath);
			xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;
			var xmlReaderSettings = new XmlReaderSettings
			{
				ValidationType = ValidationType.None,
				IgnoreWhitespace = false,
				IgnoreComments = true
			};
			using (var xmlReader = XmlReader.Create(xmlTextReader, xmlReaderSettings))
			{
				var index = 0;
				while (xmlReader.Read())
				{
					switch (xmlReader.NodeType)
					{
						case XmlNodeType.Element:
							if (index == 0 && string.Compare(xmlReader.Name, "xliff", StringComparison.OrdinalIgnoreCase) == 0)
							{
								index++;
								while (xmlReader.MoveToNextAttribute())
								{
									if (string.Compare(xmlReader.Name, "version", StringComparison.OrdinalIgnoreCase) == 0)
									{
										xliff.Version = xmlReader.Value;
									}
									else if (string.Compare(xmlReader.Name, NsPrefix + ":support", StringComparison.OrdinalIgnoreCase) == 0)
									{
										var success = Enum.TryParse(xmlReader.Value, true, out Enumerators.XLIFFSupport support);
										xliff.Support = success ? support : Enumerators.XLIFFSupport.none;
									}
									else if (string.Compare(xmlReader.Name, NsPrefix + ":version", StringComparison.OrdinalIgnoreCase) == 0)
									{
										xliff.SpecificVersion = xmlReader.Value;
									}
								}
							}
							else if (string.Compare(xmlReader.Name, NsPrefix + ":doc-info", StringComparison.OrdinalIgnoreCase) == 0)
							{
								var xmlReaderSub = xmlReader.ReadSubtree();
								xliff.DocInfo = ReadDocInfo(xmlReaderSub);
								xmlReaderSub.Close();
							}
							else if (string.Compare(xmlReader.Name, "file", StringComparison.OrdinalIgnoreCase) == 0)
							{
								var xmlReaderSub = xmlReader.ReadSubtree();
								xliff.Files.Add(ReadFile(xmlReaderSub));
								xmlReaderSub.Close();
							}
							break;
					}
				}
			}

			return xliff;
		}

		private DocInfo ReadDocInfo(XmlReader xmlReader)
		{
			var docInfo = new DocInfo();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, NsPrefix + ":doc-info", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "project-id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									docInfo.ProjectId = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "source", StringComparison.OrdinalIgnoreCase) == 0)
								{
									docInfo.Source = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "source-language", StringComparison.OrdinalIgnoreCase) == 0)
								{
									docInfo.SourceLanguage = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "target-language", StringComparison.OrdinalIgnoreCase) == 0)
								{
									docInfo.TargetLanguage = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "created", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
									var success = DateTime.TryParseExact(xmlReader.Value, format,
										CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue);

									docInfo.Created = success ? dateValue : DateTime.MinValue;
								}
							}
						}
						else if (string.Compare(xmlReader.Name, NsPrefix + ":cmt-defs", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							docInfo.Comments = ReadCommentDefinitions(xmlReaderSub);
							xmlReaderSub.Close();
						}
						break;

				}
			}

			return docInfo;
		}

		private Dictionary<string, List<IComment>> ReadCommentDefinitions(XmlReader xmlReader)
		{
			var commentDefinitions = new Dictionary<string, List<IComment>>();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, NsPrefix + ":cmt-defs", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
						}
						else if (string.Compare(xmlReader.Name, NsPrefix + ":cmt-def", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							var item = ReadCommentDefinition(xmlReaderSub);
							if (!string.IsNullOrEmpty(item.Key))
							{
								commentDefinitions.Add(item.Key, item.Value);
							}

							xmlReaderSub.Close();
						}
						break;
				}
			}

			return commentDefinitions;
		}

		private KeyValuePair<string, List<IComment>> ReadCommentDefinition(XmlReader xmlReader)
		{
			var id = string.Empty;
			var commentDefinition = new KeyValuePair<string, List<IComment>>();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, NsPrefix + ":cmt-def", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									id = xmlReader.Value;
								}
							}
						}
						else if (string.Compare(xmlReader.Name, NsPrefix + ":comments", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							commentDefinition = new KeyValuePair<string, List<IComment>>(id, ReadComments(xmlReaderSub));
							xmlReaderSub.Close();
						}

						break;
				}
			}

			return commentDefinition;
		}

		private List<IComment> ReadComments(XmlReader xmlReader)
		{
			var comments = new List<IComment>();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, NsPrefix + ":comments", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
						}
						else if (string.Compare(xmlReader.Name, NsPrefix + ":comment", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							comments.Add(ReadComment(xmlReaderSub));
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return comments;
		}

		private IComment ReadComment(XmlReader xmlReader)
		{
			var user = string.Empty;
			var version = string.Empty;
			var date = DateTime.MinValue;
			var severity = Severity.Undefined;
			var text = string.Empty;

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, NsPrefix + ":comment", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "user", StringComparison.OrdinalIgnoreCase) == 0)
								{
									user = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "version", StringComparison.OrdinalIgnoreCase) == 0)
								{
									version = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "date", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
									var success = DateTime.TryParseExact(xmlReader.Value, format,
										CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue);

									date = success ? dateValue : DateTime.MinValue;
								}
								if (string.Compare(xmlReader.Name, "severity", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = Enum.TryParse<Severity>(xmlReader.Value, true, out var value);
									severity = success ? value : Severity.Undefined;
								}
							}
						}
						break;
					case XmlNodeType.Whitespace:
						text += xmlReader.Value;
						break;
					case XmlNodeType.Text:
						text += xmlReader.Value;
						break;
					case XmlNodeType.CDATA:
						text += xmlReader.Value;
						break;
					case XmlNodeType.EntityReference:
						text += xmlReader.Name;
						break;
				}
			}

			var comment = _segmentBuilder.CreateComment(text, user, severity, date, version);

			return comment;
		}

		private File ReadFile(XmlReader xmlReader)
		{
			var file = new File();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "file", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "original", StringComparison.OrdinalIgnoreCase) == 0)
								{
									file.Original = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "source-language", StringComparison.OrdinalIgnoreCase) == 0)
								{
									file.SourceLanguage = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "target-language", StringComparison.OrdinalIgnoreCase) == 0)
								{
									file.TargetLanguage = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "datatype", StringComparison.OrdinalIgnoreCase) == 0)
								{
									file.DataType = xmlReader.Value;
								}
							}
						}
						else if (string.Compare(xmlReader.Name, "body", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							file.Body = ReadBody(xmlReaderSub);
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return file;
		}

		private Body ReadBody(XmlReader xmlReader)
		{
			var body = new Body();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (string.Compare(xmlReader.Name, "group", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							body.TransUnits.Add(ReadGroup(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "trans-unit", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							body.TransUnits.Add(ReadTransUnit(xmlReaderSub));
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return body;
		}

		private TransUnit ReadGroup(XmlReader xmlReader)
		{
			var transUnit = new TransUnit();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "group", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
						}
						else if (string.Compare(xmlReader.Name, "trans-unit", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							transUnit = ReadTransUnit(xmlReaderSub);
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return transUnit;
		}

		private TransUnit ReadTransUnit(XmlReader xmlReader)
		{
			var transUnit = new TransUnit();
			var sourceSegments = new List<Source>();
			var targetSegments = new List<Target>();
			var segmentPairProperties = new List<ISegmentPairProperties>();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "trans-unit", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									transUnit.Id = xmlReader.Value;
								}
							}
						}
						if (string.Compare(xmlReader.Name, "seg-source", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							sourceSegments = ReadSourceSegments(xmlReaderSub);
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "target", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							targetSegments = ReadTargetSegments(xmlReaderSub);
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, NsPrefix + ":seg-defs", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segmentPairProperties = ReadSegmentPairPropertyDefinitions(xmlReaderSub);
							xmlReaderSub.Close();
						}

						break;
				}
			}

			foreach (var sourceSegment in sourceSegments)
			{
				var segmentPair = new SegmentPair(_segmentBuilder);
				segmentPair.Id = sourceSegment.Id;
				segmentPair.Source.Elements.AddRange(sourceSegment.Elements);

				var targetSegment = targetSegments.FirstOrDefault(a => a.Id.Equals(sourceSegment.Id));
				if (targetSegment != null)
				{
					segmentPair.Target.Elements.AddRange(targetSegment.Elements);
				}

				var properties = segmentPairProperties.FirstOrDefault(a => a.Id.Id == segmentPair.Id);
				if (properties != null)
				{
					segmentPair.ConfirmationLevel = properties.ConfirmationLevel;
					segmentPair.IsLocked = properties.IsLocked;
					segmentPair.TranslationOrigin = properties.TranslationOrigin;
				}

				transUnit.SegmentPairs.Add(segmentPair);
			}

			return transUnit;
		}

		private List<ISegmentPairProperties> ReadSegmentPairPropertyDefinitions(XmlReader xmlReader)
		{
			var segmentPairProperties = new List<ISegmentPairProperties>();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (string.Compare(xmlReader.Name, NsPrefix + ":seg", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segmentPairProperties.Add(ReadSdlSeg(xmlReaderSub));
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return segmentPairProperties;
		}


		private List<Source> ReadSourceSegments(XmlReader xmlReader)
		{
			var segments = new List<Source>();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (string.Compare(xmlReader.Name, "mrk", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							var segment = ReadSourceSegment(xmlReaderSub);
							segments.Add(segment);
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return segments;
		}

		private List<Target> ReadTargetSegments(XmlReader xmlReader)
		{
			var segments = new List<Target>();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (string.Compare(xmlReader.Name, "mrk", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							var segment = ReadTargetSegment(xmlReaderSub);
							segments.Add(segment);
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return segments;
		}

		private Source ReadSourceSegment(XmlReader xmlReader)
		{
			var segment = new Source();
			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "mrk", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "mid", StringComparison.OrdinalIgnoreCase) == 0)
								{
									segment.Id = xmlReader.Value;
								}
							}
						}
						if (string.Compare(xmlReader.Name, "bpt", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segment.Elements.Add(ReadOpeningElementTagPair(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "ept", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segment.Elements.Add(ReadClosingElementTagPair(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "ph", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segment.Elements.Add(ReadElementPlaceholder(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "x", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segment.Elements.Add(ReadElementGenericPlaceholder(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "mrk", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							var elements = ReadElements(xmlReaderSub);
							if (elements?.Count > 0)
							{
								segment.Elements.AddRange(elements);
							}

							xmlReaderSub.Close();
						}
						break;
					case XmlNodeType.Whitespace:
						{
							var whiteSpace = new ElementText
							{
								Text = xmlReader.Value
							};

							segment.Elements.Add(whiteSpace);
						}
						break;
					case XmlNodeType.Text:
						{
							var text = new ElementText
							{
								Text = xmlReader.Value
							};

							segment.Elements.Add(text);
						}
						break;
					case XmlNodeType.CDATA:
						{
							var text = new ElementText
							{
								Text = xmlReader.Value
							};

							segment.Elements.Add(text);
						}
						break;
					case XmlNodeType.EntityReference:
						{
							var text = new ElementText
							{
								Text = xmlReader.Name
							};

							segment.Elements.Add(text);
						}
						break;
				}
			}

			return segment;
		}

		private Target ReadTargetSegment(XmlReader xmlReader)
		{
			var segment = new Target();
			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "mrk", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "mid", StringComparison.OrdinalIgnoreCase) == 0)
								{
									segment.Id = xmlReader.Value;
								}
							}
						}
						if (string.Compare(xmlReader.Name, "bpt", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segment.Elements.Add(ReadOpeningElementTagPair(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "ept", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segment.Elements.Add(ReadClosingElementTagPair(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "ph", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segment.Elements.Add(ReadElementPlaceholder(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "x", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segment.Elements.Add(ReadElementGenericPlaceholder(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "mrk", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							var elements = ReadElements(xmlReaderSub);
							if (elements?.Count > 0)
							{
								segment.Elements.AddRange(elements);
							}

							xmlReaderSub.Close();
						}
						break;
					case XmlNodeType.Whitespace:
						{
							var whiteSpace = new ElementText
							{
								Text = xmlReader.Value
							};

							segment.Elements.Add(whiteSpace);
						}
						break;
					case XmlNodeType.Text:
						{
							var text = new ElementText
							{
								Text = xmlReader.Value
							};

							segment.Elements.Add(text);
						}
						break;
					case XmlNodeType.CDATA:
						{
							var text = new ElementText
							{
								Text = xmlReader.Value
							};

							segment.Elements.Add(text);
						}
						break;
					case XmlNodeType.EntityReference:
						{
							var text = new ElementText
							{
								Text = xmlReader.Name
							};

							segment.Elements.Add(text);
						}
						break;
				}
			}

			return segment;
		}

		private ElementTagPair ReadOpeningElementTagPair(XmlReader xmlReader)
		{
			var elementTagPair = new ElementTagPair
			{
				Type = Element.TagType.OpeningTag
			};

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "bpt", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									elementTagPair.TagId = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "equiv-text", StringComparison.OrdinalIgnoreCase) == 0)
								{
									elementTagPair.DisplayText = xmlReader.Value;
								}
							}
						}
						break;
					//case XmlNodeType.Whitespace:
					//	elementTagPair.TagContent += xmlReader.Value;
					//	break;
					case XmlNodeType.Text:
						elementTagPair.TagContent += xmlReader.Value;
						break;
					case XmlNodeType.CDATA:
						elementTagPair.TagContent += xmlReader.Value;
						break;
					case XmlNodeType.EntityReference:
						elementTagPair.TagContent += xmlReader.Name;
						break;
				}
			}

			return elementTagPair;
		}

		private ElementTagPair ReadClosingElementTagPair(XmlReader xmlReader)
		{
			var elementTagPair = new ElementTagPair
			{
				Type = Element.TagType.ClosingTag
			};

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "ept", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									elementTagPair.TagId = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "equiv-text", StringComparison.OrdinalIgnoreCase) == 0)
								{
									elementTagPair.DisplayText = xmlReader.Value;
								}
							}
						}
						break;
					//case XmlNodeType.Whitespace:
					//	elementTagPair.TagContent += xmlReader.Value;
					//	break;
					case XmlNodeType.Text:
						elementTagPair.TagContent += xmlReader.Value;
						break;
					case XmlNodeType.CDATA:
						elementTagPair.TagContent += xmlReader.Value;
						break;
					case XmlNodeType.EntityReference:
						elementTagPair.TagContent += xmlReader.Name;
						break;
				}
			}

			return elementTagPair;
		}

		private ElementPlaceholder ReadElementPlaceholder(XmlReader xmlReader)
		{
			var placeholder = new ElementPlaceholder();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "ph", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									placeholder.TagId = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "equiv-text", StringComparison.OrdinalIgnoreCase) == 0)
								{
									placeholder.DisplayText = xmlReader.Value;
								}
							}
						}
						break;
					//case XmlNodeType.Whitespace:
					//	placeholder.TagContent += xmlReader.Value;
					//	break;
					case XmlNodeType.Text:
						placeholder.TagContent += xmlReader.Value;
						break;
					case XmlNodeType.CDATA:
						placeholder.TagContent += xmlReader.Value;
						break;
					case XmlNodeType.EntityReference:
						placeholder.TagContent += xmlReader.Name;
						break;
				}
			}

			return placeholder;
		}

		private ElementGenericPlaceholder ReadElementGenericPlaceholder(XmlReader xmlReader)
		{
			var placeholder = new ElementGenericPlaceholder();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "x", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									placeholder.TagId = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "equiv-text", StringComparison.OrdinalIgnoreCase) == 0)
								{
									placeholder.TextEquivalent = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "ctype", StringComparison.OrdinalIgnoreCase) == 0)
								{
									placeholder.CType = xmlReader.Value;
								}
							}
						}
						break;
				}
			}

			return placeholder;
		}

		private List<Element> ReadElements(XmlReader xmlReader)
		{
			var mtype = string.Empty;
			var mid = string.Empty;
			var cid = string.Empty;

			var elements = new List<Element>();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "mrk", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "mid", StringComparison.OrdinalIgnoreCase) == 0)
								{
									mid = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "cid", StringComparison.OrdinalIgnoreCase) == 0)
								{
									cid = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "mtype", StringComparison.OrdinalIgnoreCase) == 0)
								{
									mtype = xmlReader.Value;
								}
							}
						}
						if (string.Compare(xmlReader.Name, "bpt", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							elements.Add(ReadOpeningElementTagPair(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "ept", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							elements.Add(ReadClosingElementTagPair(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "ph", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							elements.Add(ReadElementPlaceholder(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "x", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							elements.Add(ReadElementGenericPlaceholder(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "mrk", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							var subElements = ReadElements(xmlReaderSub);
							if (subElements?.Count > 0)
							{
								elements.AddRange(subElements);
							}

							xmlReaderSub.Close();
						}
						break;
					//case XmlNodeType.Whitespace:
					//	{
					//		var whiteSpace = new ElementText
					//		{
					//			Text = xmlReader.Value
					//		};

					//		elements.Add(whiteSpace);
					//	}
					//	break;
					case XmlNodeType.Text:
						{
							var text = new ElementText
							{
								Text = xmlReader.Value
							};

							elements.Add(text);
						}
						break;
					case XmlNodeType.CDATA:
						{
							var text = new ElementText
							{
								Text = xmlReader.Value
							};

							elements.Add(text);
						}
						break;
					case XmlNodeType.EntityReference:
						{
							var text = new ElementText
							{
								Text = xmlReader.Name
							};

							elements.Add(text);
						}
						break;
				}
			}


			if (string.Compare(mtype, "protected", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				elements.Insert(0, new ElementLocked
				{
					Type = Element.TagType.OpeningTag
				});
				elements.Add(new ElementLocked
				{
					Type = Element.TagType.ClosingTag
				});
			}
			if (string.Compare(mtype, "seg", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				elements.Insert(0, new ElementSegment
				{
					Id = mid,
					Type = Element.TagType.OpeningTag
				});
				elements.Add(new ElementSegment
				{
					Id = cid,
					Type = Element.TagType.ClosingTag
				});
			}
			if (string.Compare(mtype, "x-sdl-comment", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				elements.Insert(0, new ElementComment
				{
					Id = cid,
					Type = Element.TagType.OpeningTag
				});
				elements.Add(new ElementComment
				{
					Id = cid,
					Type = Element.TagType.ClosingTag
				});
			}

			return elements;
		}

		private ISegmentPairProperties ReadSdlSeg(XmlReader xmlReader)
		{
			var properties = _segmentBuilder.CreateSegmentPairProperties();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, NsPrefix + ":seg", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									properties.Id = new SegmentId(xmlReader.Value);
								}
								if (string.Compare(xmlReader.Name, "conf", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = Enum.TryParse<ConfirmationLevel>(xmlReader.Value, true, out var value);
									properties.ConfirmationLevel = success ? value : ConfirmationLevel.Unspecified;
								}
								if (string.Compare(xmlReader.Name, "locked", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = bool.TryParse(xmlReader.Value, out var result);
									properties.IsLocked = success && result;
								}
								if (string.Compare(xmlReader.Name, "origin", StringComparison.OrdinalIgnoreCase) == 0)
								{
									properties.TranslationOrigin.OriginType = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "origin-system", StringComparison.OrdinalIgnoreCase) == 0)
								{
									properties.TranslationOrigin.OriginSystem = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "percent", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = byte.TryParse(xmlReader.Value, out var result);
									properties.TranslationOrigin.MatchPercent = success ? result : (byte)0;
								}
								if (string.Compare(xmlReader.Name, "struct-match", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = bool.TryParse(xmlReader.Value, out var result);
									properties.TranslationOrigin.IsStructureContextMatch = success && result;
								}
								if (string.Compare(xmlReader.Name, "text-match", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = Enum.TryParse<TextContextMatchLevel>(xmlReader.Value, true, out var result);
									properties.TranslationOrigin.TextContextMatchLevel = success ? result : TextContextMatchLevel.None;
								}
							}
						}
						if (string.Compare(xmlReader.Name, NsPrefix + ":prev-origin", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							properties.TranslationOrigin.OriginBeforeAdaptation = ReadPreviousTranslationOrigin(xmlReaderSub);
							xmlReaderSub.Close();
						}

						break;
				}
			}

			return properties;
		}

		private ITranslationOrigin ReadPreviousTranslationOrigin(XmlReader xmlReader)
		{
			var translationOrigin = _segmentBuilder.CreateTranslationOrigin();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, NsPrefix + ":prev-origin", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "origin", StringComparison.OrdinalIgnoreCase) == 0)
								{
									translationOrigin.OriginType = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "origin-system", StringComparison.OrdinalIgnoreCase) == 0)
								{
									translationOrigin.OriginSystem = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "percent", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = byte.TryParse(xmlReader.Value, out var result);
									translationOrigin.MatchPercent = success ? result : (byte)0;
								}
								if (string.Compare(xmlReader.Name, "struct-match", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = bool.TryParse(xmlReader.Value, out var result);
									translationOrigin.IsStructureContextMatch = success && result;
								}
								if (string.Compare(xmlReader.Name, "text-match", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = Enum.TryParse<TextContextMatchLevel>(xmlReader.Value, true, out var result);
									translationOrigin.TextContextMatchLevel = success ? result : TextContextMatchLevel.None;
								}
							}
						}
						if (string.Compare(xmlReader.Name, NsPrefix + ":prev-origin", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							translationOrigin.OriginBeforeAdaptation = ReadPreviousTranslationOrigin(xmlReaderSub);
							xmlReaderSub.Close();
						}

						break;
				}
			}

			return translationOrigin;
		}

		private static string ReadText(XmlReader xmlReaderInner)
		{
			var value = string.Empty;

			while (xmlReaderInner.Read())
			{
				switch (xmlReaderInner.NodeType)
				{
					case XmlNodeType.Element:
						//not required for this implementation
						break;
					case XmlNodeType.Whitespace:
						value += xmlReaderInner.Value;
						break;
					case XmlNodeType.Text:
						value += xmlReaderInner.Value;
						break;
					case XmlNodeType.CDATA:
						value += xmlReaderInner.Value;
						break;
					case XmlNodeType.EntityReference:
						value += xmlReaderInner.Name;
						break;
					case XmlNodeType.EndElement:
						//not required for this implementation
						break;
				}
			}
			return value;

		}
	}
}
