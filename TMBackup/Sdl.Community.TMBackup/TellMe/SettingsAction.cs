using System.Drawing;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using static Sdl.Community.TMBackup.TMBackupRibbon;

namespace Sdl.Community.TMBackup.TellMe
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
			SdlTradosStudio.Application.ExecuteAction<TMBackupAction>();
		}
	}
}