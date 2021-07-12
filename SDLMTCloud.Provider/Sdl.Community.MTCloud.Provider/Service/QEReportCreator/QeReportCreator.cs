using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.Core;
using File = Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel.File;

namespace Sdl.Community.MTCloud.Provider.Service.QEReportCreator
{
	public class QeReportCreator
	{
		public string CreateXmlReport(IProject project, ProjectFile[] projectFiles, List<QeFileReport> qeFileReports)
		{
			var projectInfo = project.GetProjectInfo();
			var report = new Report
			{
				Summary = new ReportSummary
				{
					Task = "MT QE Report",  
					Project = projectInfo.Name,
					DueDate = $"{projectInfo.DueDate?.ToShortDateString()} {projectInfo.DueDate?.ToLongTimeString()}",
					Files = projectFiles.Length,
					Location = projectInfo.LocalProjectFolder,
					CreatedAt = $"{projectInfo.CreatedAt.ToShortDateString()} {projectInfo.CreatedAt.ToLongTimeString()}",
					Language = qeFileReports[0].LanguageDirection.TargetLanguage.DisplayName
				}
			};
			var data = new Data();
			foreach (var fr in qeFileReports)
			{
				var file = new File
				{
					Name = fr.FileName
				};
				foreach (var item in fr.SegmentsPerCategory)
				{
					file.QeValues.Add(new QeValue
					{
						SegmentsTotal = item.Value.Count,
						WordsTotal = GetWordsTotalInSegment(item.Value),
						QualityEstimation = item.Key
					});
				}
				data.File.Add(file);
			}
			report.Data = data;

			var stringWriter = new StringWriter();
			new XmlSerializer(typeof(Report)).Serialize(stringWriter, report);

			return stringWriter.ToString();
		}

		private static int GetWordsTotalInSegment(List<ISegmentPair> segmentPairs)
		{
			var wordsTotal = 0;
			foreach (var segment in segmentPairs)
			{
				wordsTotal += segment.Target.ToString().Split(' ').Length;
			}

			return wordsTotal;
		}
	}
}