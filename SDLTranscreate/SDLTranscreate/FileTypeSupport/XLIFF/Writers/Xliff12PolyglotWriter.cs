using System;
using System.Collections.Generic;
using System.Xml;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.Interfaces;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Writers
{
	public class Xliff12PolyglotWriter : IXliffWriter
	{
		private const string NsPrefix = "sdlxliff";
		private Dictionary<string, List<IComment>> Comments { get; set; }

		private bool IncludeTranslations { get; set; }

		public bool WriteFile(Xliff xliff, string outputFilePath, bool includeTranslations)
		{
			Comments = xliff.DocInfo.Comments;
			IncludeTranslations = includeTranslations;
			UpdateGenericPlaceholderIds(xliff);

			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = false,
				Indent = false
			};

			var version = "1.2";
			var sdlSupport = Enumerators.XLIFFSupport.xliff12polyglot.ToString();
			var sdlVersion = version + ".1";

			using (var writer = XmlWriter.Create(outputFilePath, settings))
			{
				writer.WriteStartElement("xliff");
				writer.WriteAttributeString("version", version);
				writer.WriteAttributeString("xmlns", NsPrefix, null, "http://schemas.sdl.com/xliff");
				writer.WriteAttributeString(NsPrefix, "support", null, sdlSupport);
				writer.WriteAttributeString(NsPrefix, "version", null, sdlVersion);

				WriteDocInfo(xliff, writer);

				foreach (var xliffFile in xliff.Files)
				{
					writer.WriteStartElement("file");
					writer.WriteAttributeString("original", xliffFile.Original);
					writer.WriteAttributeString("source-language", xliffFile.SourceLanguage);
					if (includeTranslations)
					{
						writer.WriteAttributeString("target-language", xliffFile.TargetLanguage);
					}

					//writer.WriteAttributeString("datatype", xliffFile.DataType);
					writer.WriteAttributeString("datatype", "xml");

					WriterFileHeader(writer, xliffFile);
					WriteFileBody(writer, xliffFile);

					writer.WriteEndElement(); // file
				}

				writer.WriteEndElement(); //xliff
			}

			return true;
		}

		private void UpdateGenericPlaceholderIds(Xliff xliff)
		{
			var lastId = 0;
			foreach (var xliffFile in xliff.Files)
			{
				foreach (var transUnit in xliffFile.Body.TransUnits)
				{
					foreach (var segmentPair in transUnit.SegmentPairs)
					{
						var ids = new List<int>();
						foreach (var element in segmentPair.Source.Elements)
						{
							if (element is ElementGenericPlaceholder genericPlaceholder)
							{
								ids.Add(++lastId);
								genericPlaceholder.TagId = string.Format("lb{0}", lastId);
							}
						}

						var targetLbIndex = 0;
						foreach (var element in segmentPair.Target.Elements)
						{
							if (element is ElementGenericPlaceholder genericPlaceholder)
							{
								int id;
								if (targetLbIndex < ids.Count)
								{
									id = ids[targetLbIndex];
								}
								else
								{
									ids.Add(++lastId);
									id = lastId;
								}

								targetLbIndex++;

								genericPlaceholder.TagId = string.Format("x{0}", id);
							}
						}
					}
				}
			}
		}

		private void WriteDocInfo(Xliff xliff, XmlWriter writer)
		{
			writer.WriteStartElement(NsPrefix, "doc-info", null);
			writer.WriteAttributeString("project-id", xliff.DocInfo.ProjectId);
			writer.WriteAttributeString("document-id", xliff.DocInfo.DocumentId);
			writer.WriteAttributeString("source", xliff.DocInfo.Source);
			writer.WriteAttributeString("source-language", xliff.DocInfo.SourceLanguage);
			writer.WriteAttributeString("target-language", xliff.DocInfo.TargetLanguage);
			writer.WriteAttributeString("created", GetDateToString(xliff.DocInfo.Created));

			writer.WriteEndElement(); //doc-info
		}

		private void WriteFileBody(XmlWriter writer, File xliffFile)
		{
			writer.WriteStartElement("body");
			foreach (var transUnit in xliffFile.Body.TransUnits)
			{
				// Polyglot flavor
				WriteGroupPolyglot(writer, transUnit);
			}

			writer.WriteEndElement(); // body
		}

		private void WriteGroupPolyglot(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("group");
			writer.WriteAttributeString("id", transUnit.Id);

			WriteTransUnitPolytlot(writer, transUnit);
			WriteSdlSegCtxs(writer, transUnit);

			writer.WriteEndElement(); // group
		}

		private void WriteSdlSegCtxs(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement(NsPrefix, "seg-cxts", null);

			foreach (var context in transUnit.Contexts)
			{
				WriteSdlSegCtx(writer, context);
			}

			writer.WriteEndElement(); //sdl:seg-cxts
		}

		private void WriteSdlSegCtx(XmlWriter writer, Context context)
		{
			writer.WriteStartElement(NsPrefix, "cxt", null);

			writer.WriteAttributeString("id", context.Id);

			writer.WriteEndElement(); //sdl:cxt
		}
		
		private void WriteTransUnitPolytlot(XmlWriter writer, TransUnit transUnit)
		{
			foreach (var segmentPair in transUnit.SegmentPairs)
			{
				var originType = segmentPair.TranslationOrigin != null ? segmentPair.TranslationOrigin.OriginType : string.Empty;
				var originSystem = segmentPair.TranslationOrigin != null ? segmentPair.TranslationOrigin.OriginSystem : string.Empty;
				var matchPercentage = segmentPair.TranslationOrigin?.MatchPercent.ToString() ?? "0";
				var structMatch = segmentPair.TranslationOrigin?.IsStructureContextMatch.ToString() ?? string.Empty;
				var textMatch = segmentPair.TranslationOrigin?.TextContextMatchLevel.ToString();

				writer.WriteStartElement("trans-unit");
				writer.WriteAttributeString("id", segmentPair.Id);
				if (segmentPair.IsLocked)
				{
					writer.WriteAttributeString(NsPrefix, "locked", null, segmentPair.IsLocked.ToString());
				}

				if (IncludeTranslations)
				{
					writer.WriteAttributeString(NsPrefix, "conf", null, segmentPair.ConfirmationLevel.ToString());
					writer.WriteAttributeString(NsPrefix, "origin", null, originType);

					if (!string.IsNullOrEmpty(originSystem))
					{
						writer.WriteAttributeString(NsPrefix, "origin-system", null, originSystem);
					}

					if (!string.IsNullOrEmpty(matchPercentage) && matchPercentage != "0")
					{
						writer.WriteAttributeString(NsPrefix, "percent", null, matchPercentage);
					}

					if (!string.IsNullOrEmpty(structMatch) && structMatch != "False")
					{
						writer.WriteAttributeString(NsPrefix, "struct-match", null, structMatch);
					}

					if (!string.IsNullOrEmpty(textMatch) && textMatch != "None")
					{
						writer.WriteAttributeString(NsPrefix, "text-match", null, textMatch);
					}
				}

				WriteSegmentNotesPolyglot(writer, segmentPair);
				WriteSegmentPolyglot(writer, segmentPair, true);

				if (IncludeTranslations)
				{
					WriteSegmentPolyglot(writer, segmentPair, false);
				}

				writer.WriteEndElement(); // trans-unit
			}
		}

		private void WriteSegmentNotesPolyglot(XmlWriter writer, SegmentPair segmentPair)
		{
			var sourceComments = GetSegmentComments(segmentPair.Source.Elements);
			WriteNotes(writer, sourceComments, "source");

			if (IncludeTranslations)
			{
				var targetComments = GetSegmentComments(segmentPair.Target.Elements);
				WriteNotes(writer, targetComments, "target");
			}
		}

		private void WriteNotes(XmlWriter writer, Dictionary<string, List<IComment>> comments, string annotates)
		{
			foreach (var commentKeyPair in comments)
			{
				foreach (var comment in commentKeyPair.Value)
				{
					writer.WriteStartElement("note");
					writer.WriteAttributeString(NsPrefix, "id", null, commentKeyPair.Key);
					writer.WriteAttributeString(NsPrefix, "version", null, comment.Version);
					if (comment.DateSpecified)
					{
						writer.WriteAttributeString(NsPrefix, "date", null, GetDateToString(comment.Date));
					}

					writer.WriteAttributeString("from", comment.Author);
					writer.WriteAttributeString("priority", GetPriority(comment.Severity).ToString());
					writer.WriteAttributeString("annotates", annotates);

					writer.WriteString(comment.Text);

					writer.WriteEndElement(); // note
				}
			}
		}

		private void WriteSegmentPolyglot(XmlWriter writer, SegmentPair segmentPair, bool isSource)
		{
			writer.WriteStartElement(isSource ? "source" : "target");
			//writer.WriteAttributeString("xml", "space", null, "preserve");

			var elements = isSource ? segmentPair.Source.Elements : segmentPair.Target.Elements;

			foreach (var element in elements)
			{
				WriteSegment(writer, element);
			}

			writer.WriteEndElement(); // source or target
		}

		private void WriteSegment(XmlWriter writer, Element element)
		{
			if (element is ElementText text)
			{
				writer.WriteString(text.Text);
			}

			if (element is ElementTagPair tag)
			{
				switch (tag.Type)
				{
					case Element.TagType.TagOpen:
						writer.WriteStartElement("bpt");
						writer.WriteAttributeString("id", tag.TagId);
						if (!string.IsNullOrEmpty(tag.DisplayText))
						{
							writer.WriteAttributeString("equiv-text", tag.DisplayText);
						}
						writer.WriteString(tag.TagContent);
						writer.WriteEndElement();
						break;
					case Element.TagType.TagClose:
						writer.WriteStartElement("ept");
						writer.WriteAttributeString("id", tag.TagId);
						if (!string.IsNullOrEmpty(tag.DisplayText))
						{
							writer.WriteAttributeString("equiv-text", tag.DisplayText);
						}
						writer.WriteString(tag.TagContent);
						writer.WriteEndElement();
						break;
				}
			}

			if (element is ElementPlaceholder placeholder)
			{
				writer.WriteStartElement("ph");
				writer.WriteAttributeString("id", placeholder.TagId);
				if (!string.IsNullOrEmpty(placeholder.DisplayText))
				{
					writer.WriteAttributeString("equiv-text", placeholder.DisplayText);
				}
				writer.WriteString(placeholder.TagContent);
				writer.WriteEndElement();
			}

			if (element is ElementGenericPlaceholder genericPlaceholder)
			{
				writer.WriteStartElement("x");
				writer.WriteAttributeString("id", genericPlaceholder.TagId);
				writer.WriteAttributeString("ctype", genericPlaceholder.CType);
				if (!string.IsNullOrEmpty(genericPlaceholder.TextEquivalent))
				{
					writer.WriteAttributeString("equiv-text", genericPlaceholder.TextEquivalent);
				}
				writer.WriteEndElement();
			}

			if (element is ElementLocked locked)
			{
				switch (locked.Type)
				{
					case Element.TagType.TagOpen:
						writer.WriteStartElement("mrk");
						writer.WriteAttributeString("mtype", "protected");
						break;
					case Element.TagType.TagClose:
						writer.WriteEndElement();
						break;
				}
			}

			if (element is ElementComment comment)
			{
				switch (comment.Type)
				{
					case Element.TagType.TagOpen:
						writer.WriteStartElement("mrk");
						writer.WriteAttributeString("mtype", "x-sdl-comment");
						writer.WriteAttributeString("cid", comment.Id);
						break;
					case Element.TagType.TagClose:
						writer.WriteEndElement();
						break;
				}
			}
		}

		private void WriterFileHeader(XmlWriter writer, File xliffFile)
		{
			writer.WriteStartElement("header");
			if (!string.IsNullOrEmpty(xliffFile.Header?.Skl?.ExternalFile?.Uid))
			{
				writer.WriteStartElement("skl");

				writer.WriteStartElement("external-file");
				writer.WriteAttributeString("uid", xliffFile.Header.Skl.ExternalFile.Uid);
				writer.WriteAttributeString("href", xliffFile.Header.Skl.ExternalFile.Href);
				writer.WriteEndElement(); // external-file

				writer.WriteEndElement(); // skl
			}
			
			WriterContextDefinitions(writer, xliffFile);

			writer.WriteEndElement(); // header
		}

		private void WriterContextDefinitions(XmlWriter writer, File xliffFile)
		{
			writer.WriteStartElement("cxt-defs");

			foreach (var context in xliffFile.Header.Contexts)
			{
				writer.WriteStartElement("cxt-def");
				writer.WriteAttributeString("id", context.Id);
				writer.WriteAttributeString("type", context.ContextType);
				writer.WriteAttributeString("code", context.DisplayCode?.Trim());
				writer.WriteAttributeString("name", context.DisplayName?.Trim());
				writer.WriteAttributeString("descr", context.Description);

				writer.WriteStartElement("props");
				foreach (var metaData in context.MetaData)
				{
					writer.WriteStartElement("value");
					writer.WriteAttributeString("key", metaData.Key);
					writer.WriteString(metaData.Value);
					writer.WriteEndElement(); // value
				}
				writer.WriteEndElement(); // props

				writer.WriteEndElement(); // cxt-def
			}

			writer.WriteEndElement(); // cxt-defs
		}

		private string GetDateToString(DateTime date)
		{
			var value = string.Empty;

			if (date != DateTime.MinValue || date != DateTime.MaxValue)
			{
				return date.ToUniversalTime()
					.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
			}

			return value;
		}

		private int GetPriority(Severity severity)
		{
			switch (severity)
			{
				case Severity.High:
					return 3;
				case Severity.Medium:
					return 2;
				case Severity.Low:
					return 1;
			}

			return 0;
		}

		private Dictionary<string, List<IComment>> GetSegmentComments(IEnumerable<Element> elements)
		{
			var comments = new Dictionary<string, List<IComment>>();
			foreach (var element in elements)
			{
				if (element is ElementComment comment && comment.Type == Element.TagType.TagOpen)
				{
					if (!Comments.ContainsKey(comment.Id))
					{
						continue;
					}

					if (comments.ContainsKey(comment.Id))
					{
						comments[comment.Id].AddRange(Comments[comment.Id]);
					}
					else
					{
						comments.Add(comment.Id, Comments[comment.Id]);
					}
				}
			}

			return comments;
		}
	}
}
