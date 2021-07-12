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
		public Dictionary<string, QeFileReport> QeFileReports { get; set; }

		public QeLabelExtractor(ProjectFile projectFile, Dictionary<string, QeFileReport> qeFileReports)
		{
			_projectFile = projectFile;
			QeFileReports = qeFileReports;
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
				var qualityEstimation = segmentPair.Properties.TranslationOrigin?.GetMetaData("quality_estimation");
				if (string.IsNullOrEmpty(qualityEstimation)) continue;

				var projectFileLocalFilePath = _projectFile.LocalFilePath;

				if (QeFileReports.TryGetValue(projectFileLocalFilePath, out var qeFileReport))
				{
					if (qeFileReport.SegmentsPerCategory.TryGetValue(qualityEstimation, out var segmentsOfCurrentQuality))
					{
						segmentsOfCurrentQuality.Add(segmentPair);
					}
					else
					{
						qeFileReport.SegmentsPerCategory[qualityEstimation] = new List<ISegmentPair> {segmentPair};
					}
				}
				else
				{
					QeFileReports[projectFileLocalFilePath] = new QeFileReport
					{
						LanguageDirection = _projectFile.GetLanguageDirection(),
						FileName = _projectFile.Name
					};
					QeFileReports[projectFileLocalFilePath].SegmentsPerCategory[qualityEstimation].Add(segmentPair);
				}
			}
		}
	}
}