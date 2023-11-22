using Sdl.Community.MTEdge.Provider.XliffConverter.Models;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.Converter
{
	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2")]
	public class Xliff
	{
		[XmlAttribute]
		public string Version { get; set; }

		[XmlElement("file")]
		public File File { get; set; }

		public void AddSourceText(string sourceText)
		{
			if (sourceText is null)
			{
				throw new NullReferenceException("Source text cannot be null");
			}

			var segment = new Segment();
			segment.Add(sourceText);
			File?.Body?.Add(segment);
		}

		public Segment[] GetTargetSegments()
		{
			var segments = new List<Segment>();
			foreach (var tu in File.Body.TranslationUnits)
			{
                if (tu.TranslationList?.FirstOrDefault()?.Translation?.TargetSegment is not Segment segment)
				{
					return null;
				}

				segment.Culture = File.TargetCulture ?? File.SourceCulture;
				segments.Add(segment);
			}

			return segments.ToArray();
		}

        public void AddSourceSegment(Segment sourceSegment)
			=> File?.Body?.Add(sourceSegment);

        public void AddTranslation(Segment sourceSegment, Segment targetSegment, string toolId)
			=> File?.Body?.Add(sourceSegment, targetSegment, toolId);

        public void AddTranslationUnit(LanguagePlatform.TranslationMemory.TranslationUnit translationUnit, string toolId)
			=> File?.Body?.Add(translationUnit?.SourceSegment, translationUnit?.TargetSegment, toolId);

        public override string ToString()
			=> Converter.PrintXliff(this);
    }
}