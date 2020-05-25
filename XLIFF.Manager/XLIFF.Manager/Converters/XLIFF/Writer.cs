using System;
using System.Collections.Generic;
using System.Xml;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Converters.XLIFF.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.Converters.XLIFF
{
	public class Writer
	{
		private Enumerators.XLIFFSupport XLIFFSupport { get; set; }
		private Dictionary<string, List<IComment>> XLIFFComments { get; set; }
		private bool IncludeTranslations { get; set; }

		public bool CreateXliffFile(Xliff xliff, string outputFilePath, Enumerators.XLIFFSupport xliffSupport, bool includeTranslations)
		{
			XLIFFComments = xliff.Comments;
			XLIFFSupport = xliffSupport;
			IncludeTranslations = includeTranslations;

			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = true
			};

			var version = "1.2";
			var sdlSupport = xliffSupport.ToString();
			var sdlVersion = version;

			using (var writer = XmlWriter.Create(outputFilePath, settings))
			{
				writer.WriteStartElement("xliff");
				writer.WriteAttributeString("version", version);
				writer.WriteAttributeString("xmlns", "sdl", null, "http://schemas.sdl.com/xliff");
				writer.WriteAttributeString("sdl", "support", null, sdlSupport);
				writer.WriteAttributeString("sdl", "version", null, sdlVersion);

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

					writer.WriteAttributeString("datatype", xliffFile.DataType);

					WriterFileHeader(writer, xliffFile);
					WriteFileBody(writer, xliffFile);

					writer.WriteEndElement(); // file
				}

				writer.WriteEndElement(); //xliff
			}

			return true;
		}

		private void WriteDocInfo(Xliff xliff, XmlWriter writer)
		{
			writer.WriteStartElement("sdl", "doc-info", null);
			writer.WriteAttributeString("project-id", xliff.DocInfo.ProjectId);
			writer.WriteAttributeString("source", xliff.DocInfo.Source);
			writer.WriteAttributeString("source-language", xliff.DocInfo.SourceLanguage);
			if (IncludeTranslations)
			{
				writer.WriteAttributeString("target-language", xliff.DocInfo.TargetLanguage);
			}

			writer.WriteAttributeString("created", GetDateToString(xliff.DocInfo.Created));

			// only supported by SDL flavor of XLIFF
			if (XLIFFSupport == Enumerators.XLIFFSupport.xliff12sdl)
			{
				WriteCommentDefinitions(xliff, writer);
			}

			writer.WriteEndElement(); //doc-info
		}

		private void WriteCommentDefinitions(Xliff xliff, XmlWriter writer)
		{
			writer.WriteStartElement("sdl", "cmt-defs", null);
			foreach (var comments in xliff.Comments)
			{
				WriteCommentDefinition(writer, comments);
			}

			writer.WriteEndElement(); //cmt-defs
		}

		private void WriteCommentDefinition(XmlWriter writer, KeyValuePair<string, List<IComment>> comments)
		{
			writer.WriteStartElement("sdl", "cmt-def", null);
			writer.WriteAttributeString("id", comments.Key);

			WriteComments(writer, comments);

			writer.WriteEndElement(); //cmt-def
		}

		private void WriteComments(XmlWriter writer, KeyValuePair<string, List<IComment>> comments)
		{
			writer.WriteStartElement("sdl", "comments", null);
			foreach (var comment in comments.Value)
			{
				WriteComment(writer, comment);
			}

			writer.WriteEndElement(); //comments
		}

		private void WriteComment(XmlWriter writer, IComment comment)
		{
			writer.WriteStartElement("sdl", "comment", null);
			writer.WriteAttributeString("user", comment.Author);
			writer.WriteAttributeString("date", GetDateToString(comment.Date));
			writer.WriteAttributeString("version", comment.Version);
			writer.WriteAttributeString("severity", comment.Severity.ToString());
			writer.WriteString(comment.Text);
			writer.WriteEndElement(); //comment
		}

		private void WriteFileBody(XmlWriter writer, File xliffFile)
		{
			writer.WriteStartElement("body");
			foreach (var transUnit in xliffFile.Body.TransUnits)
			{
				if (XLIFFSupport == Enumerators.XLIFFSupport.xliff12polyglot)
				{
					// Polyglot flavor
					WriteGroupPolyglot(writer, transUnit);
				}
				else
				{
					// SDL flavor
					WriteTransUnit(writer, transUnit);
				}
			}
			writer.WriteEndElement(); // body
		}

		private void WriteGroupPolyglot(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("group");
			writer.WriteAttributeString("id", transUnit.Id);

			WriteTransUnitPolytlot(writer, transUnit);

			writer.WriteEndElement(); // group
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
					writer.WriteAttributeString("sdl", "locked", null, segmentPair.IsLocked.ToString());
				}

				if (IncludeTranslations)
				{
					writer.WriteAttributeString("sdl", "conf", null, segmentPair.ConfirmationLevel.ToString());
					writer.WriteAttributeString("sdl", "origin", null, originType);

					if (!string.IsNullOrEmpty(originSystem))
					{
						writer.WriteAttributeString("sdl", "origin-system", null, originSystem);
					}

					if (!string.IsNullOrEmpty(matchPercentage) && matchPercentage != "0")
					{
						writer.WriteAttributeString("sdl", "percent", null, matchPercentage);
					}

					if (!string.IsNullOrEmpty(structMatch) && structMatch != "False")
					{
						writer.WriteAttributeString("sdl", "struct-match", null, structMatch);
					}

					if (!string.IsNullOrEmpty(textMatch) && textMatch != "None")
					{
						writer.WriteAttributeString("sdl", "text-match", null, textMatch);
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
					writer.WriteAttributeString("sdl", "id", null, commentKeyPair.Key);
					writer.WriteAttributeString("sdl", "version", null, comment.Version);
					if (comment.DateSpecified)
					{
						writer.WriteAttributeString("sdl", "date", null, GetDateToString(comment.Date));
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
			if (segmentPair.IsLocked)
			{
				writer.WriteStartElement("mrk");
				writer.WriteAttributeString("mtype", "protected");
			}

			var elements = isSource ? segmentPair.Source.Elements : segmentPair.Target.Elements;

			foreach (var element in elements)
			{
				WriteSegment(writer, element);
			}

			if (segmentPair.IsLocked)
			{
				writer.WriteEndElement(); // mrk
			}

			writer.WriteEndElement(); // source or target
		}

		private void WriteTransUnit(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("trans-unit");
			writer.WriteAttributeString("id", transUnit.Id);

			WriteSourceParagraph(writer, transUnit);
			WriteSegSource(writer, transUnit);
			WriteTargetParagraph(writer, transUnit);
			WriteSdlSegDefs(writer, transUnit);

			writer.WriteEndElement(); // trans-unit
		}

		private void WriteSdlSegDefs(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("sdl", "seg-defs", null);

			foreach (var segmentPair in transUnit.SegmentPairs)
			{
				WriteSdlSeg(writer, segmentPair);
			}

			writer.WriteEndElement(); //sdl:seg-defs
		}

		private void WriteSdlSeg(XmlWriter writer, SegmentPair segmentPair)
		{
			writer.WriteStartElement("sdl", "seg", null);
			writer.WriteAttributeString("id", segmentPair.Id);
			writer.WriteAttributeString("conf", segmentPair.ConfirmationLevel.ToString());

			if (segmentPair.IsLocked)
			{
				writer.WriteAttributeString("locked", segmentPair.IsLocked.ToString());
			}

			if (segmentPair.TranslationOrigin != null)
			{
				WriteTranslationOrigin(writer, segmentPair.TranslationOrigin);

				if (segmentPair.TranslationOrigin?.OriginBeforeAdaptation != null)
				{
					writer.WriteStartElement("sdl", "prev-origin", null);
					WriteTranslationOrigin(writer, segmentPair.TranslationOrigin?.OriginBeforeAdaptation);
					writer.WriteEndElement(); //sdl:prev-origin
				}
			}

			writer.WriteEndElement(); //sdl:seg
		}

		private static void WriteTranslationOrigin(XmlWriter writer, ITranslationOrigin translationOrigin)
		{
			var originType = translationOrigin != null ? translationOrigin.OriginType : string.Empty;
			var originSystem = translationOrigin != null ? translationOrigin.OriginSystem : string.Empty;
			var matchPercentage = translationOrigin?.MatchPercent.ToString() ?? "0";
			var structMatch = translationOrigin?.IsStructureContextMatch.ToString() ?? string.Empty;
			var textMatch = translationOrigin?.TextContextMatchLevel != null 
				? translationOrigin.TextContextMatchLevel.ToString() 
				: string.Empty;

			if (!string.IsNullOrEmpty(originType))
			{
				writer.WriteAttributeString("origin", originType);
			}

			if (!string.IsNullOrEmpty(originSystem))
			{
				writer.WriteAttributeString("origin-system", originSystem);
			}

			if (!string.IsNullOrEmpty(matchPercentage) && matchPercentage != "0")
			{
				writer.WriteAttributeString("percent", matchPercentage);
			}

			if (!string.IsNullOrEmpty(structMatch) && structMatch != "False")
			{
				writer.WriteAttributeString("struct-match", structMatch);
			}

			if (!string.IsNullOrEmpty(textMatch) && textMatch != "None")
			{
				writer.WriteAttributeString("text-match", textMatch);
			}

			if (translationOrigin?.MetaData != null)
			{
				foreach (var keyValuePair in translationOrigin.MetaData)
				{
					writer.WriteStartElement("sdl", "value", null);

					writer.WriteAttributeString("key", keyValuePair.Key);
					writer.WriteString(keyValuePair.Value);

					writer.WriteEndElement(); //sdl:value
				}
			}
		}

		private void WriteSourceParagraph(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("source");
			for (var index = 0; index < transUnit.SegmentPairs.Count; index++)
			{
				var segmentPair = transUnit.SegmentPairs[index];

				if (index > 0)
				{
					var addSpace = AddSpaceBetweenSegmentationPosition(transUnit, index);
					if (addSpace)
					{
						writer.WriteString(" ");
					}
				}

				foreach (var element in segmentPair.Source.Elements)
				{
					WriteSegment(writer, element);
				}
			}

			writer.WriteEndElement(); // source
		}

		private void WriteSegSource(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("seg-source");
			foreach (var segmentPair in transUnit.SegmentPairs)
			{
				writer.WriteStartElement("mrk");
				writer.WriteAttributeString("mtype", "seg");
				writer.WriteAttributeString("mid", segmentPair.Id);

				if (segmentPair.IsLocked)
				{
					writer.WriteStartElement("mrk");
					writer.WriteAttributeString("mtype", "protected");
				}

				foreach (var element in segmentPair.Source.Elements)
				{
					WriteSegment(writer, element);
				}

				if (segmentPair.IsLocked)
				{
					writer.WriteEndElement(); // mrk
				}

				writer.WriteEndElement(); // mrk
			}

			writer.WriteEndElement(); // seg-source
		}

		private void WriteTargetParagraph(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("target");

			foreach (var segmentPair in transUnit.SegmentPairs)
			{
				writer.WriteStartElement("mrk");
				writer.WriteAttributeString("mtype", "seg");
				writer.WriteAttributeString("sdl", "state", null, GetState(segmentPair.ConfirmationLevel));
				writer.WriteAttributeString("mid", segmentPair.Id);

				if (segmentPair.IsLocked)
				{
					writer.WriteStartElement("mrk");
					writer.WriteAttributeString("mtype", "protected");
				}

				foreach (var element in segmentPair.Target.Elements)
				{
					WriteSegment(writer, element);
				}

				if (segmentPair.IsLocked)
				{
					writer.WriteEndElement(); // mrk
				}


				writer.WriteEndElement(); // mrk
			}

			writer.WriteEndElement(); // seg-source
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
					case Element.TagType.OpeningTag:
						writer.WriteStartElement("bpt");
						writer.WriteAttributeString("id", tag.TagId);
						writer.WriteString(tag.TagContent);
						writer.WriteEndElement();
						break;
					case Element.TagType.ClosingTag:
						writer.WriteStartElement("ept");
						writer.WriteAttributeString("id", tag.TagId);
						writer.WriteString(tag.TagContent);
						writer.WriteEndElement();
						break;
				}
			}

			if (element is ElementPlaceholder placeholder)
			{
				writer.WriteStartElement("ph");
				writer.WriteAttributeString("id", placeholder.TagId);
				writer.WriteString(placeholder.TagContent);
				writer.WriteEndElement();
			}

			if (element is ElementLocked locked)
			{
				writer.WriteStartElement("mrk");
				writer.WriteAttributeString("type", "protected");
				writer.WriteString(locked.TagContent);
				writer.WriteEndElement();
			}

			if (element is ElementComment comment)
			{
				switch (comment.Type)
				{
					case Element.TagType.OpeningTag:
						writer.WriteStartElement("mrk");
						writer.WriteAttributeString("mtype", "x-sdl-comment");
						writer.WriteAttributeString("cid", comment.Id);
						break;
					case Element.TagType.ClosingTag:
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
			writer.WriteEndElement(); // header
		}

		private bool AddSpaceBetweenSegmentationPosition(TransUnit transUnit, int index)
		{
			var addSpace = true;

			var foundSpaceStart = false;
			var foundSpaceEnd = false;

			var currentFirstElement = transUnit.SegmentPairs[index].Source.Elements[0];
			if (currentFirstElement is ElementText text1)
			{
				foundSpaceStart = text1.Text.StartsWith(" ");
			}

			var previous = transUnit.SegmentPairs[index - 1].Source;
			var previousLastElement = previous.Elements[previous.Elements.Count - 1];
			if (previousLastElement is ElementText text2)
			{
				foundSpaceEnd = text2.Text.EndsWith(" ");
			}

			if (foundSpaceStart || foundSpaceEnd)
			{
				addSpace = false;
			}

			return addSpace;
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

		private string GetState(ConfirmationLevel confirmationLevel)
		{
			var state = string.Empty;

			switch (confirmationLevel)
			{
				case ConfirmationLevel.Unspecified:
					state = "new";
					break;
				case ConfirmationLevel.Draft:
					state = "needs-translation";
					break;
				case ConfirmationLevel.Translated:
					state = "translated";
					break;
				case ConfirmationLevel.RejectedTranslation:
					state = "needs-review-translation";
					break;
				case ConfirmationLevel.ApprovedTranslation:
					state = "signed-off";
					break;
				case ConfirmationLevel.RejectedSignOff:
					state = "needs-review-translation";
					break;
				case ConfirmationLevel.ApprovedSignOff:
					state = "final";
					break;
			}

			return state;
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
				if (element is ElementComment comment && comment.Type == Element.TagType.OpeningTag)
				{
					if (!XLIFFComments.ContainsKey(comment.Id))
					{
						continue;
					}

					if (comments.ContainsKey(comment.Id))
					{
						comments[comment.Id].AddRange(XLIFFComments[comment.Id]);
					}
					else
					{
						comments.Add(comment.Id, XLIFFComments[comment.Id]);
					}
				}
			}

			return comments;
		}
	}
}
