using System;
using System.Collections.Generic;
using Sdl.Community.TQA.Providers;

namespace Sdl.Community.TQA.Model
{
	public class Entry
	{
		public string Language { get; set; }
		public string File { get; set; }
		public string Segment { get; set; }
		public string OriginalTranslation { get; set; }
		public List<Tuple<string, TextType>> RevisedTranslation { get; set; }
		public string HighlightedText { get; set; }
		public List<Tuple<string, TextType>> SourceContent { get; set; }
		public string Category { get; set; }
		public string Severity { get; set; }
		public string Subcategory { get; set; }
		public string Comment { get; set; }

		public Entry(string language, string file, string segment, string originalTranslation,
			List<Tuple<string, TextType>> revisedTranslation, List<Tuple<string, TextType>> sourceContent,
			string category, string subcategory, string severity,
			string comment, string highlightedText)
		{
			Language = language;
			File = file;
			Segment = segment;
			OriginalTranslation = originalTranslation;
			RevisedTranslation = revisedTranslation;
			SourceContent = sourceContent;
			Category = category;
			Subcategory = subcategory;

			switch (severity)
			{
				case "Minor weight":
					Severity = "Minor Error";
					break;
				case "Serious weight":
					Severity = "Serious Error";
					break;
				default:
					Severity = severity;
					break;
			}
			Comment = comment;
			HighlightedText = highlightedText;
		}

		internal string[] GetArray(TQAProfileType tqaStandardType)
		{   if (tqaStandardType==TQAProfileType.tqsMQM)
			  return new[] { File, Segment, "", OriginalTranslation, "", Category, Subcategory, Severity, Comment };
		    else if (tqaStandardType==TQAProfileType.tqsJ2450)
			  return new[] { File, Segment, "", OriginalTranslation, "", Category, Severity, Comment };
			throw new NotSupportedException(string.Format(PluginResources.MsgTQAProfileStandardNotSupported, tqaStandardType));
		}
	}
}
