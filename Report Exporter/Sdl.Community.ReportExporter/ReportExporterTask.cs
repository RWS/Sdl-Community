using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.ReportExporter.Helpers;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ReportExporter
{
	/// <summary>
	/// Automatic task attribute
	/// Parameters: task name, plugin name and plugin description
	/// </summary>
	[AutomaticTask("Export analysis report",
		"Export analysis report",
		"Exports a report  in CSV format",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(ReportExporterSettings), typeof(ReportExporterSettingsPage))]
	
	public class ReportExporterTask: AbstractFileContentProcessingAutomaticTask
	{
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{

			var projects = GetSetting<ReportExporterSettings>().ProjectsList;
			foreach (var project in projects)
			{
				// check which languages to export
				var checkedLanguages = project.LanguagesForPoject.Where(c => c.Value);
				foreach (var languageReport in checkedLanguages)
				{
					var csvFullReportPath =
						languageReport.Key.PathToReport.Substring(0, languageReport.Key
							                                             .PathToReport.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

					//write report to Reports folder
					using (var sw = new StreamWriter(csvFullReportPath + Path.DirectorySeparatorChar +
					                                 languageReport.Key.TargetLang.Name + ".csv"))
					{
						var report = new StudioAnalysisReport(languageReport.Key.PathToReport);
						sw.Write(report.ToCsv(true));
					}

				}
			}
			//foreach (var project in projects)
			//{
			//	var report = new StudioAnalysisReport(project);
			//	//Clipboard.SetText(report.ToCsv(true));
			//	Thread thread = new Thread(() => Clipboard.SetText(report.ToCsv(true)));
			//	thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
			//	thread.Start();
			//	thread.Join();
			//}
		}
	}
}
