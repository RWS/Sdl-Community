using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.MTCloud.Provider.Studio.QEReportBatchTask
{
	public class QeLabelExtractor : AbstractBilingualContentProcessor
	{
		private readonly ProjectFile _projectFile;
		private readonly WordCounter _wordCounter;

		public Dictionary<string, QeFileReport> QeFileReports { get; set; }

		public QeLabelExtractor(ProjectFile projectFile, Dictionary<string, QeFileReport> qeFileReports, WordCounter wordCounter)
		{
			_projectFile = projectFile;
			QeFileReports = qeFileReports;
			_wordCounter = wordCounter;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure)
			{
				return;
			}

			var segmentPairs = paragraphUnit.SegmentPairs.ToList();

			foreach (var segmentPair in segmentPairs)
			{
				string qualityEstimation = null;
				var translationOrigin = segmentPair.Properties.TranslationOrigin;
				if (translationOrigin.OriginSystem == PluginResources.OriginSystem_LWC)
				{
					qualityEstimation = translationOrigin.GetMetaData("quality_estimation");
				}
				if (string.IsNullOrEmpty(qualityEstimation))
					continue;

				var projectFileLocalFilePath = _projectFile.LocalFilePath;

				if (QeFileReports.TryGetValue(projectFileLocalFilePath, out var qeFileReport))
				{
					if (qeFileReport.SegmentsPerCategory.TryGetValue(qualityEstimation, out var segmentsOfCurrentQuality))
					{
						segmentsOfCurrentQuality.Item1.Add(segmentPair);
						segmentsOfCurrentQuality.Item2.Increment(GetSegmentWordCount(segmentPair));
					}
					else
					{
						qeFileReport.SegmentsPerCategory[qualityEstimation] = Tuple.Create(new List<ISegmentPair>(), GetSegmentWordCount(segmentPair)).ToValueTuple();
					}
				}
				else
				{
					QeFileReports[projectFileLocalFilePath] = new QeFileReport
					{
						LanguageDirection = _projectFile.GetLanguageDirection(),
						FileName = _projectFile.Name
					};
					QeFileReports[projectFileLocalFilePath].SegmentsPerCategory[qualityEstimation].Item1.Add(segmentPair);
					QeFileReports[projectFileLocalFilePath].SegmentsPerCategory[qualityEstimation].Item2.Increment(GetSegmentWordCount(segmentPair));
				}
			}
		}

		private CountData GetSegmentWordCount(ISegmentPair segmentPair)
		{
			return _wordCounter.Count(segmentPair.Source);
		}
	}
}