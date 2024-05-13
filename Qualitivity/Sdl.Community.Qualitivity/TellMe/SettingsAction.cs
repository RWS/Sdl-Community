using System.Diagnostics;
using System.Drawing;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.SettingsQ;

		public SettingsAction()
        {
            Name = string.Format("{0} settings", PluginResources.Plugin_Name);
        }

		public override void Execute()
		{
			SdlTradosStudio.Application.ExecuteAction<QualitivityConfiguration>();
		}
	}
}
