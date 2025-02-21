using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
	public class SettingsAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Settings;

		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} Settings";
		}

		public override void Execute()
		{
			SdlTradosStudio.Application.GetAction<ApplyStudioProjectTemplateAction>().RunOptions();
		}
	}
}
