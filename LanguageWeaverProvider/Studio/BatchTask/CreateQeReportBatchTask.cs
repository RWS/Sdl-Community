using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using LanguageWeaverProvider.Studio.BatchTask.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using System.Linq;

namespace LanguageWeaverProvider.Studio.BatchTask
{
	[AutomaticTask(Id = "Create QE Report",
	Name = "Create QE Report",
	Description = "Create a QE Report for files translated by Language Weaver provider.",
	GeneratedFileType = AutomaticTaskFileType.BilingualTarget,
	AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(CreateQeReportSettings), typeof(CreateQeReportSettingsPage))]
	public class CreateQEReportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		readonly List<CreateQeReportProcessor> _segments = new();
		CreateQeReportSettings _settings;

		protected override void OnInitializeTask()
		{
			_settings = GetSetting<CreateQeReportSettings>();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var fileName = System.IO.Path.GetFileName(projectFile.LocalFilePath);
			var wordCounter = GetWordCounter(projectFile);

			var processor = new CreateQeReportProcessor(projectFile.GetLanguageDirection(), fileName, wordCounter, _settings);
			var processorHandler = new BilingualContentHandlerAdapter(processor);
			multiFileConverter?.AddBilingualProcessor(processorHandler);
			multiFileConverter?.Parse();

			_segments.Add(processor);
		}

		public override void TaskComplete()
		{
			base.TaskComplete();
			var report = BuildXMLReport();
			CreateStudioReport(report);
		}

		private Report BuildXMLReport()
		{
			var data = new Data();
			foreach (var segment in _segments)
			{
				var fileName = $"{segment.FileName}: {segment.LanguageDirection.SourceLanguage.DisplayName} - {segment.LanguageDirection.TargetLanguage.DisplayName}";
				var qeValues = segment.Segments.Select(qs => new QeValue()
				{
					QualityEstimation = qs.Key,
					SegmentsTotal = qs.Value.QeCount,
					WordsTotal = qs.Value.WordsCount,
					CharactersTotal = qs.Value.CharacterCount
				}).ToList();

				var file = new File()
				{
					Name = fileName,
					QeValues = qeValues
				};

				data.File.Add(file);
			}

			var projectInfo = Project.GetProjectInfo();
			var reportSummary = new ReportSummary
			{
				Task = "MT QE Report",
				Project = projectInfo.Name,
				DueDate = $"{projectInfo.DueDate?.ToString("g")}",
				CreatedAt = $"{projectInfo.CreatedAt:g}",
				Files = _segments.Count,
				Location = projectInfo.LocalProjectFolder,
				LockedSegmentsAreExcluded = _settings.ExcludeLockedSegments ? "Yes" : "No"
			};

			var report = new Report
			{
				Summary = reportSummary,
				Data = data
			};

			return report;
		}

		private void CreateStudioReport(Report report)
		{
			var xmlReport = SerializeReportToXml(report);
			CreateReport("Segments", "Segments' evaluations", xmlReport);
		}

		private string SerializeReportToXml(Report report)
		{
			using var stringWriter = new System.IO.StringWriter();
			using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Encoding = Encoding.UTF8 });
			new XmlSerializer(typeof(Report)).Serialize(xmlWriter, report);
			return stringWriter.ToString();
		}
	}
}