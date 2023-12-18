using System.Collections.Generic;
using LanguageWeaverProvider.Studio.BatchTask.Model;
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
				[NoQE] = new(),
				[QualityEstimations.None.ToString()] = new(),
				[QualityEstimations.Poor.ToString()] = new(),
				[QualityEstimations.Adequate.ToString()] = new(),
				[QualityEstimations.Good.ToString()] = new(),
			};
		}

		public string FileName { get; private set; }

		public Dictionary<string, ReportSegmentDetails> Segments { get; private set; }

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
				Segments[qualityEstimation].QeCouunt++;
				Segments[qualityEstimation].WordsCount += _wordCounter.Count(pair.Target).Words;
			}
		}

		private string GetCurrentQEValue(ISegmentPair segmentPair)
		{
			var qualityEstimation = segmentPair.Properties?.TranslationOrigin?.GetMetaData(Constants.SegmentMetadata_QE);
			return string.IsNullOrEmpty(qualityEstimation) ? NoQE : qualityEstimation;
		}
	}
}