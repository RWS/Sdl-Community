using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Sdl.Community.MTCloud.Provider.XliffConverter.Models;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.XliffConverter.Converter
{
	public class EvaluatedSegment
	{
		public string QualityEstimation { get; set; }
		public Segment Segment { get; set; }
	}

	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2")]
	public class Xliff
	{
		[XmlElement("file")]
		public File File { get; set; }

		[XmlAttribute]
		public string Version { get; set; }

		public void AddSourceSegment(Segment sourceSegment)
		{
			File?.Body?.Add(sourceSegment);
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

		public void AddTranslation(Segment sourceSegment, Segment targetSegment, string toolId)
		{
			File?.Body?.Add(sourceSegment, targetSegment, toolId);
		}

		public void AddTranslationUnit(LanguagePlatform.TranslationMemory.TranslationUnit translationUnit, string toolId)
		{
			File?.Body?.Add(translationUnit?.SourceSegment, translationUnit?.TargetSegment, toolId);
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
}