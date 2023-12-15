using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.AutomaticTasks;

namespace LanguageWeaverProvider.Studio.BatchTask
{
	public class CreateQeReportProcessor : AbstractBilingualContentProcessor
	{
		private const string NoQE = "No QE set";
		private readonly WordCounter _wordCounter;

		public CreateQeReportProcessor(string fileName, WordCounter wordCounter)
		{
			_wordCounter = wordCounter;
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
			base.ProcessParagraphUnit(paragraphUnit);

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

				var x = _wordCounter.Count(pair.Target);
			}
		}

		private string GetCurrentQEValue(ISegmentPair segmentPair)
		{
			var qualityEstimation = segmentPair.Properties?.TranslationOrigin?.GetMetaData(Constants.SegmentMetadata_QE);
			return string.IsNullOrEmpty(qualityEstimation) ? NoQE : qualityEstimation;
		}
	}
}