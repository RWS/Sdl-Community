using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel;
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
					Task = "QE Report",
					Project = projectInfo.Name,
					DueDate = projectInfo.DueDate,
					Files = projectFiles.Length,
					Location = projectInfo.LocalProjectFolder,
					CreatedAt = projectInfo.CreatedAt,
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
						Total = item.Value.Count,
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
	}
}