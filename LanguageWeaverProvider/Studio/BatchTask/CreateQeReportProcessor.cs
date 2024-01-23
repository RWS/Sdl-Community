using System.Collections.Generic;
using LanguageWeaverProvider.Studio.BatchTask.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.AutomaticTasks;

namespace LanguageWeaverProvider.Studio.BatchTask
{
	public class CreateQeReportProcessor : AbstractBilingualContentProcessor
	{
		const string NoQE = "No QE set";
		readonly WordCounter _wordCounter;
		readonly CreateQeReportSettings _settings;

		public CreateQeReportProcessor(LanguageDirection languageDirection, string fileName, WordCounter wordCounter, CreateQeReportSettings settings)
		{
			_settings = settings;
			_wordCounter = wordCounter;
			FileName = fileName;
			LanguageDirection = languageDirection;
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

		public LanguageDirection LanguageDirection { get; private set; }

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
				var skipSegment = _settings.ExcludeLockedSegments && pair.Properties.IsLocked;
				if (skipSegment)
				{
					continue;
				}

				var counter = _wordCounter.Count(pair.Target);
				var qualityEstimation = GetCurrentQEValue(pair);
				Segments[qualityEstimation].QeCount++;
				Segments[qualityEstimation].WordsCount += counter.Words;
				Segments[qualityEstimation].CharacterCount += counter.Characters;
			}
		}

		private string GetCurrentQEValue(ISegmentPair segmentPair)
		{
			var qualityEstimation = segmentPair.Properties?.TranslationOrigin?.GetMetaData(Constants.SegmentMetadata_QE);
			return string.IsNullOrEmpty(qualityEstimation) ? NoQE : qualityEstimation;
		}
	}
}