using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Sdl.Community.MTEdge.Provider.XliffConverter.Models;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.Converter
{
	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2")]
	public class Xliff
	{
		[XmlAttribute]
		public string Version { get; set; }

		[XmlElement("file")]
		public File File { get; set; }


		public void AddTranslationUnit(LanguagePlatform.TranslationMemory.TranslationUnit translationUnit, string toolId)
		{
			File?.Body?.Add(translationUnit?.SourceSegment, translationUnit?.TargetSegment, toolId);
		}

		public void AddSourceText(string sourceText)
		{
			if (sourceText == null)
			{
				throw new NullReferenceException("Source text cannot be null");
			}

			var seg = new Segment();
			seg.Add(sourceText);
			File?.Body?.Add(seg);
		}

		public void AddSourceSegment(Segment sourceSegment)
		{
			File?.Body?.Add(sourceSegment);
		}

		public void AddTranslation(Segment sourceSegment, Segment targetSegment, string toolId)
		{
			File?.Body?.Add(sourceSegment, targetSegment, toolId);
		}

		public Segment[] GetTargetSegments(out List<string> sources)
		{
			sources = new List<string>();
			var segments = new List<Segment>();
			foreach (var tu in File.Body.TranslationUnits)
			{
				var option = tu.TranslationList?.FirstOrDefault();
				var segment = option?.Translation?.TargetSegment;

				if (segment == null)
				{
					return null;
				}

				sources.Add(tu.SourceText);
				segment.Culture = File.TargetCulture ?? File.SourceCulture;
				segments.Add(segment);
			}

			return segments.ToArray();
		}

		public override string ToString()
		{
			return Converter.PrintXliff(this);
		}
	}
}
