using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
	internal class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = string.Format("{0} - TM Anonymizer Settings", PluginResources.Plugin_Name);
		}

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.TellmeDocumentation;
		public override bool IsAvailable => true;

		public override void Execute() => ApplicationContext.ShowSettingsWindow();
	}
}