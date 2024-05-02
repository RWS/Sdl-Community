using System.Diagnostics;
using System.Drawing;
using Sdl.Community.ExportAnalysisReports;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => "Trados Export Analysis Reports results";

		public override Icon Icon => PluginResources.Settings;

		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} settings";
		}

		public override void Execute()
		{
			SdlTradosStudio.Application.GetAction<AbstractExportReportAction>().RunSettingsView();
		}
	}
}
