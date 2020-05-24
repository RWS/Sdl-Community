using System;
using System.Collections.Generic;
using System.Xml;
using Sdl.Community.XLIFF.Manager.Converters.XLIFF.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.Converters.XLIFF
{
	public class Writer
	{
		public bool CreateXliffFile(Xliff xliff, string outputFilePath)
		{
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = true
			};

			using (var writer = XmlWriter.Create(outputFilePath, settings))
			{
				writer.WriteStartElement("xliff");
				writer.WriteAttributeString("version", "1.2");
				writer.WriteAttributeString("xmlns", "sdl", null, "http://schemas.sdl.com/xliff");

				WriteDocInfo(xliff, writer);

				foreach (var xliffFile in xliff.Files)
				{
					writer.WriteStartElement("file");
					writer.WriteAttributeString("original", xliffFile.Original);
					writer.WriteAttributeString("source-language", xliffFile.SourceLanguage);
					//writer.WriteAttributeString("target-language", xliffFile.TargetLanguage);
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
			//writer.WriteAttributeString("target-language", xliff.DocInfo.TargetLanguage);
			writer.WriteAttributeString("created", GetDateToString(xliff.DocInfo.Created));

			WriteCommentDefinitions(xliff, writer);

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

		private static void WriteFileBody(XmlWriter writer, File xliffFile)
		{
			writer.WriteStartElement("body");
			foreach (var transUnit in xliffFile.Body.TransUnits)
			{
				WriteGroupPolyGlot(writer, transUnit);
			}
			writer.WriteEndElement(); // body
		}

		private static void WriteGroupPolyGlot(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("group");
			writer.WriteAttributeString("id", transUnit.Id);
			
			WriteSourceTransUnitPolyGlot(writer, transUnit);
			
			writer.WriteEndElement(); // group
		}

		private static void WriteSourceTransUnitPolyGlot(XmlWriter writer, TransUnit transUnit)
		{			
			foreach (var segmentPair in transUnit.SegmentPairs)
			{
				writer.WriteStartElement("trans-unit");
				writer.WriteAttributeString("id", segmentPair.Id);


				writer.WriteStartElement("source");
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

				writer.WriteEndElement(); // source


				writer.WriteEndElement(); // trans-unit
			}
		}

		private static void WriteTransUnit(XmlWriter writer, TransUnit transUnit)
		{
			writer.WriteStartElement("trans-unit");
			writer.WriteAttributeString("id", transUnit.Id);

			WriteSource(writer, transUnit);
			WriteSegSource(writer, transUnit);
			WriteTarget(writer, transUnit);

			writer.WriteEndElement(); // trans-unit
		}


		private static void WriteSource(XmlWriter writer, TransUnit transUnit)
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

		private static void WriteSegSource(XmlWriter writer, TransUnit transUnit)
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

		private static void WriteTarget(XmlWriter writer, TransUnit transUnit)
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


			//public enum ConfirmationLevel
			//{
			//	Unspecified = 0,
			//	Draft = 1,
			//	Translated = 2,
			//	RejectedTranslation = 3,
			//	ApprovedTranslation = 4,
			//	RejectedSignOff = 5,
			//	ApprovedSignOff = 6
			//}

			writer.WriteEndElement(); // seg-source
		}

		private static string GetState(ConfirmationLevel confirmationLevel)
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

		private static void WriteSegment(XmlWriter writer, Element element)
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


		private static void WriterFileHeader(XmlWriter writer, File xliffFile)
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

		private static bool AddSpaceBetweenSegmentationPosition(TransUnit transUnit, int index)
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


	}
}
