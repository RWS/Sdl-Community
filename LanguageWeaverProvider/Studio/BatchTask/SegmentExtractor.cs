using System.Collections.Generic;
using System.Drawing.Text;
using System.Windows.Input;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace LanguageWeaverProvider.Studio.BatchTask
{
	public class SegmentExtractor : AbstractBilingualContentProcessor
	{
		private const string NoQE = "No QE set";

		public SegmentExtractor(string fileName)
		{
			FileName = fileName;
			Segments = new()
			{
				[NoQE] = 0,
				[QualityEstimations.None.ToString()] = 0,
				[QualityEstimations.Poor.ToString()] = 0,
				[QualityEstimations.Adequate.ToString()] = 0,
				[QualityEstimations.Good.ToString()] = 0,
			};
		}

		public string FileName { get; private set; }

		public Dictionary<string, int> Segments { get; private set; }

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				return;
			}

			var segmentPairs = paragraphUnit.SegmentPairs;
			if (segmentPairs is null)
			{
				return;
			}

			foreach (var pair in segmentPairs)
			{
				var qualityEstimation = GetCurrentQEValue(pair);
				Segments[qualityEstimation]++;
			}
		}

		private string GetCurrentQEValue(ISegmentPair segmentPair)
		{
			var qualityEstimation = segmentPair.Properties?.TranslationOrigin?.GetMetaData(Constants.SegmentMetadata_QE);
			return string.IsNullOrEmpty(qualityEstimation) ? NoQE : qualityEstimation;
		}
	}
}
