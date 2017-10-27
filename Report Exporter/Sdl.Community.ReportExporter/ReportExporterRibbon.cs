using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.ReportExporter.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ReportExporter
{
	[RibbonGroup("Sdl.Community.ReportExporter", Name = "Report Exporter")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]

	public class ReportExporterRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.ReportExporter", Name = "Report Exporter", Icon = "folder2_blue", Description = "Studio Report Exporter")]
	[ActionLayout(typeof(ReportExporterRibbon), 20, DisplayType.Large)]
	class StudioMigrationUtilityViewPartAction : AbstractAction
	{

		protected override void Execute()
		{
			var exporter = new ReportExporterControl();
			exporter.ShowDialog();
		}
	}

	//protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
	//	{
	//		var settings = GetSetting<ReportExporterSettings>();
	//		foreach (var project in settings.ProjectsList)
	//		{
	//			// check which languages to export
	//			var checkedLanguages = project.LanguagesForPoject.Where(c => c.Value);
	//			foreach (var languageReport in checkedLanguages)
	//			{
	//				var csvFullReportPath =
	//					languageReport.Key.PathToReport.Substring(0, languageReport.Key
	//						                                             .PathToReport.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

	//				//write report to Reports folder
	//				using (var sw = new StreamWriter(csvFullReportPath + Path.DirectorySeparatorChar +
	//				                                 languageReport.Key.TargetLang.Name + ".csv"))
	//				{
	//					var report = new StudioAnalysisReport(languageReport.Key.PathToReport);
	//					sw.Write(report.ToCsv(settings.IncludeHeader));
	//				}

	//			}
	//		}
	//	}
	//}
}
