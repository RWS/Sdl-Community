using System.Drawing;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe.Actions
{
	public class ExportAnalysisReportAction : ExportAnalysisReportAbstractTellMeAction
	{
		public ExportAnalysisReportAction()
		{
			Name = PluginResources.Plugin_Name;
		}

		public override Icon Icon => PluginResources.Settings;

		public override void Execute() => SdlTradosStudio.Application?.GetAction<ReportExporter>().ExecuteAction();
	}
}