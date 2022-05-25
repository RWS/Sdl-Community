using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.Interfaces;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Readers
{
	public class Xliff12PolyglotReader : IXliffReader
	{
		private const string NsPrefix = "sdlxliff";
		private readonly SegmentBuilder _segmentBuilder;

		private Dictionary<string, List<IComment>> Comments { get; set; }

		public Xliff12PolyglotReader(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}

		public Xliff ReadXliff(string filePath)
		{
			var xliff = new Xliff();

			var xmlTextReader = new XmlTextReader(filePath);
			var xmlReaderSettings = new XmlReaderSettings
			{
				ValidationType = ValidationType.None,
				IgnoreWhitespace = true,
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

			xliff.DocInfo.Comments = Comments;

			UpdateTransUnitContextInfo(xliff);
			
			return xliff;
		}

		private static void UpdateTransUnitContextInfo(Xliff xliff)
		{
			foreach (var file in xliff.Files)
			{
				foreach (var transUnit in file.Body.TransUnits)
				{
					if (transUnit.Contexts.Count <= 0)
					{
						continue;
					}

					foreach (var context in transUnit.Contexts)
					{
						var existingContext = file.Header.Contexts.FirstOrDefault(a => a.Id == context.Id);
						if (existingContext != null)
						{
							context.ContextType = existingContext.ContextType;
							context.DisplayName = existingContext.DisplayName;
							context.DisplayCode = existingContext.DisplayCode;
							context.Description = existingContext.Description;
							context.MetaData = existingContext.MetaData;
						}
					}
				}
			}
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
								if (string.Compare(xmlReader.Name, "document-id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									docInfo.DocumentId = xmlReader.Value;
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
						break;
				}
			}

			return docInfo;
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
						else if (string.Compare(xmlReader.Name, "header", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							file.Header = ReadHeader(xmlReaderSub);
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return file;
		}

		private Header ReadHeader(XmlReader xmlReader)
		{
			var header = new Header();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (string.Compare(xmlReader.Name, "cxt-defs", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							header.Contexts = ReadContextDefinitions(xmlReaderSub);
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return header;
		}

		private List<Context> ReadContextDefinitions(XmlReader xmlReader)
		{
			var contexts = new List<Context>();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "cxt-defs", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							//ignore
						}
						else if (string.Compare(xmlReader.Name, "cxt-def", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							contexts.Add(ReadContextDefinition(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "props", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							contexts[contexts.Count - 1].MetaData = ReadContextProperties(xmlReaderSub);
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return contexts;
		}

		private Context ReadContextDefinition(XmlReader xmlReader)
		{
			var context = new Context();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "cxt-def", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.Id = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "type", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.ContextType = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.DisplayName = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "code", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.DisplayCode = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "descr", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.Description = xmlReader.Value;
								}
							}
						}
						break;
				}
			}

			return context;
		}

		private Dictionary<string, string> ReadContextProperties(XmlReader xmlReader)
		{
			var dict = new Dictionary<string, string>();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "props", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							// ignore
						}

						if (string.Compare(xmlReader.Name, "value", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							var kvp = ReadContextMetaData(xmlReaderSub);
							dict.Add(kvp.Key, kvp.Value);
							xmlReaderSub.Close();
						}
						break;

				}
			}

			return dict;
		}

		private KeyValuePair<string, string> ReadContextMetaData(XmlReader xmlReader)
		{
			var key = string.Empty;
			var value = string.Empty;

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "value", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "key", StringComparison.OrdinalIgnoreCase) == 0)
								{
									key = xmlReader.Value;
								}
							}
						}
						break;
					case XmlNodeType.Text:
						{
							value += xmlReader.Value;
						}
						break;
				}
			}

			return new KeyValuePair<string, string>(key, value);
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
						break;
				}
			}

			return body;
		}

		private TransUnit ReadGroup(XmlReader xmlReader)
		{
			var transUnit = new TransUnit();
			var contexts = new List<Context>();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "group", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									// for polyglot support, the paragraph id is represented by the group id									
									transUnit.Id = xmlReader.Value;
								}
							}
						}
						else if (string.Compare(xmlReader.Name, "trans-unit", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							transUnit.SegmentPairs.Add(ReadTransUnit(xmlReaderSub));
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, NsPrefix + ":seg-cxts", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							contexts = ReadTransUnitContexts(xmlReaderSub);
							xmlReaderSub.Close();
						}
						break;
				}
			}
			
			transUnit.Contexts = contexts;

			return transUnit;
		}

		private List<Context> ReadTransUnitContexts(XmlReader xmlReader)
		{
			var contexts = new List<Context>();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (string.Compare(xmlReader.Name, NsPrefix + ":cxt", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							contexts.Add(ReadSdlCtx(xmlReaderSub));
							xmlReaderSub.Close();
						}
						break;
				}
			}

			return contexts;
		}

		private Context ReadSdlCtx(XmlReader xmlReader)
		{
			var context = new Context();

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, NsPrefix + ":cxt", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.Id = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "type", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.ContextType = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.DisplayName = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "code", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.DisplayCode = xmlReader.Value;
								}
								else if (string.Compare(xmlReader.Name, "descr", StringComparison.OrdinalIgnoreCase) == 0)
								{
									context.Description = xmlReader.Value;
								}
							}
						}
						break;
				}
			}

			return context;
		}

		private SegmentPair ReadTransUnit(XmlReader xmlReader)
		{
			var segmentPair = new SegmentPair(_segmentBuilder);

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
									// for polyglot support, the segment id is represented by the trans-unit id
									segmentPair.Id = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":conf", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = Enum.TryParse<ConfirmationLevel>(xmlReader.Value, true, out var value);
									segmentPair.ConfirmationLevel = success ? value : ConfirmationLevel.Unspecified;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":locked", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = bool.TryParse(xmlReader.Value, out var result);
									segmentPair.IsLocked = success && result;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":origin", StringComparison.OrdinalIgnoreCase) == 0)
								{
									segmentPair.TranslationOrigin.OriginType = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":origin-system", StringComparison.OrdinalIgnoreCase) == 0)
								{
									segmentPair.TranslationOrigin.OriginSystem = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":percent", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = byte.TryParse(xmlReader.Value, out var result);
									segmentPair.TranslationOrigin.MatchPercent = success ? result : (byte)0;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":struct-match", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = bool.TryParse(xmlReader.Value, out var result);
									segmentPair.TranslationOrigin.IsStructureContextMatch = success && result;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":text-match", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = Enum.TryParse<TextContextMatchLevel>(xmlReader.Value, true, out var result);
									segmentPair.TranslationOrigin.TextContextMatchLevel = success ? result : TextContextMatchLevel.None;
								}
							}
						}
						else if (string.Compare(xmlReader.Name, "source", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segmentPair.Source = ReadSource(xmlReaderSub);
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "target", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							segmentPair.Target = ReadTarget(xmlReaderSub);
							xmlReaderSub.Close();
						}
						else if (string.Compare(xmlReader.Name, "note", StringComparison.OrdinalIgnoreCase) == 0)
						{
							var xmlReaderSub = xmlReader.ReadSubtree();
							var comment = ReadComment(xmlReaderSub, out var id);

							AddCommentToCollection(id, comment);

							xmlReaderSub.Close();
						}
						break;
				}
			}

			return segmentPair;
		}

		private void AddCommentToCollection(string id, IComment comment)
		{
			if (Comments == null)
			{
				Comments = new Dictionary<string, List<IComment>>();
			}

			if (Comments.ContainsKey(id))
			{
				var commentDef = Comments[id];
				commentDef.Add(comment);
			}
			else
			{
				Comments.Add(id, new List<IComment> { comment });
			}
		}

		private IComment ReadComment(XmlReader xmlReader, out string id)
		{
			id = string.Empty;
			var user = string.Empty;
			var version = string.Empty;
			var date = DateTime.MinValue;
			var severity = Severity.Undefined;
			var text = string.Empty;
			var annotates = string.Empty; // currently not used

			var index = 0;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						if (index == 0 && string.Compare(xmlReader.Name, "note", StringComparison.OrdinalIgnoreCase) == 0)
						{
							index++;
							while (xmlReader.MoveToNextAttribute())
							{
								if (string.Compare(xmlReader.Name, NsPrefix + ":id", StringComparison.OrdinalIgnoreCase) == 0)
								{
									id = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, "from", StringComparison.OrdinalIgnoreCase) == 0)
								{
									user = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":version", StringComparison.OrdinalIgnoreCase) == 0)
								{
									version = xmlReader.Value;
								}
								if (string.Compare(xmlReader.Name, NsPrefix + ":date", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
									var success = DateTime.TryParseExact(xmlReader.Value, format,
										CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue);

									date = success ? dateValue : DateTime.MinValue;
								}
								if (string.Compare(xmlReader.Name, "priority", StringComparison.OrdinalIgnoreCase) == 0)
								{
									var success = int.TryParse(xmlReader.Value, out var priority);
									severity = success ? GetSeverity(priority) : Severity.Undefined;
								}
								if (string.Compare(xmlReader.Name, "annotates", StringComparison.OrdinalIgnoreCase) == 0)
								{
									annotates = xmlReader.Value;
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

		private Severity GetSeverity(int priority)
		{
			switch (priority)
			{
				case 5:
				case 4:
				case 3:
					return Severity.High;
				case 2:
					return Severity.Medium;
				case 1:
					return Severity.Low;
			}

			return Severity.Undefined;
		}

		private Source ReadSource(XmlReader xmlReader)
		{
			var segment = new Source();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
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

		private Target ReadTarget(XmlReader xmlReader)
		{
			var segment = new Target();

			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
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
				Type = Element.TagType.TagOpen
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
					case XmlNodeType.Whitespace:
						elementTagPair.TagContent += xmlReader.Value;
						break;
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
				Type = Element.TagType.TagClose
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
					case XmlNodeType.Whitespace:
						elementTagPair.TagContent += xmlReader.Value;
						break;
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
					case XmlNodeType.Whitespace:
						placeholder.TagContent += xmlReader.Value;
						break;
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
					case XmlNodeType.Whitespace:
						{
							var whiteSpace = new ElementText
							{
								Text = xmlReader.Value
							};

							elements.Add(whiteSpace);
						}
						break;
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
					Type = Element.TagType.TagOpen
				});
				elements.Add(new ElementLocked
				{
					Type = Element.TagType.TagClose
				});
			}
			if (string.Compare(mtype, "seg", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				elements.Insert(0, new ElementSegment
				{
					Id = mid,
					Type = Element.TagType.TagOpen
				});
				elements.Add(new ElementSegment
				{
					Id = cid,
					Type = Element.TagType.TagClose
				});
			}
			if (string.Compare(mtype, "x-sdl-comment", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				elements.Insert(0, new ElementComment
				{
					Id = cid,
					Type = Element.TagType.TagOpen
				});
				elements.Add(new ElementComment
				{
					Id = cid,
					Type = Element.TagType.TagClose
				});
			}

			return elements;
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
