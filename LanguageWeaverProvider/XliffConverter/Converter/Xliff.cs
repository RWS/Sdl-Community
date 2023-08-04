using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using LanguageWeaverProvider.XliffConverter.Models;
using Sdl.LanguagePlatform.Core;
using TranslationUnit = Sdl.LanguagePlatform.TranslationMemory.TranslationUnit;

namespace LanguageWeaverProvider.XliffConverter.Converter
{
	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2")]
	public class Xliff
	{
		[XmlAttribute]
		public string Version { get; set; }

		[XmlElement("file")]
		public File File { get; set; }


		public void AddTranslationUnit(TranslationUnit translationUnit, string toolId)
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

		public List<EvaluatedSegment> GetTargetSegments()
		{
			var segments = new List<EvaluatedSegment>();
			foreach (var tu in File.Body.TranslationUnits)
			{
				var option = tu.TranslationList?.FirstOrDefault();
				var segment = option?.Translation?.TargetSegment;

				if (segment == null)
				{
					return null;
				}

				segment.Culture = File.TargetCulture ?? File.SourceCulture;
				segments.Add(new EvaluatedSegment
				{
					Segment = segment,
					QualityEstimation = option.MatchQuality
				});
			}

			return segments;
		}

		public override string ToString()
		{
			return Converter.PrintXliff(this);
		}
	}

	public class EvaluatedSegment
	{
		public Segment Segment { get; set; }
		public string QualityEstimation { get; set; }
	}
}
