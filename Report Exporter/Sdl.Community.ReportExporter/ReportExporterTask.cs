using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.ProjectAutomation.AutomaticTasks;

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
	public class ReportExporterTask
	{
	}
}
