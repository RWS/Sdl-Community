using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using LanguageWeaverProvider.Studio.BatchTask.Model;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace LanguageWeaverProvider.Studio.BatchTask
{
	[AutomaticTask(Id = "Create QE Report",
	Name = "Create QE Report",
	Description = "Create a QE Report for files translated by Language Weaver provider.",
	GeneratedFileType = AutomaticTaskFileType.BilingualTarget,
	AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	public class QEReportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		private readonly List<SegmentExtractor> _segments = new();

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var fileName = System.IO.Path.GetFileName(projectFile.LocalFilePath);

			var segmentExtractor = new SegmentExtractor(fileName);
			multiFileConverter.AddBilingualProcessor(segmentExtractor);
			multiFileConverter.Parse();

			_segments.Add(segmentExtractor);
		}

		public override void TaskComplete()
		{
			var projectInfo = Project.GetProjectInfo();
			var x = projectInfo.TargetLanguages[0];
			Project.GetProjectStatistics();
			var report = new Report
			{
				Summary = new ReportSummary
				{
					Task = "MT QE Report",
					Project = projectInfo.Name,
					DueDate = $"{projectInfo.DueDate?.ToShortDateString()} {projectInfo.DueDate?.ToLongTimeString()}",
					Files = _segments.Count,
					Location = projectInfo.LocalProjectFolder,
					CreatedAt = $"{projectInfo.CreatedAt.ToShortDateString()} {projectInfo.CreatedAt.ToLongTimeString()}"
				}
			};

			var data = new Data();
			foreach (var segment in _segments)
			{
				var file = new File(segment.FileName);
				foreach (var qs in segment.Segments)
				{
					file.QeValues.Add(new QeValue()
					{
						SegmentsTotal = qs.Value,
						QualityEstimation = qs.Key
					});
				}

				data.File.Add(file);
			}

			report.Data = data;
			var stringWriter = new EncodedStringWriter(Encoding.UTF8);
			new XmlSerializer(typeof(Report)).Serialize(stringWriter, report);

			var xmlReport = stringWriter.ToString();
			CreateReport("Segments", "Segments' evaluations", xmlReport);
		}
	}
}