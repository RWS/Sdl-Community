using System.Drawing;
using Reports.Viewer.Plus.Actions;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Reports.Viewer.Plus.TellMe
{
	internal class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} settings";
		}

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			SdlTradosStudio.Application.GetController<ReportsViewerController>().Initialize();
			SdlTradosStudio.Application.GetAction<OpenSettingsAction>().Run();
		}
	}
}