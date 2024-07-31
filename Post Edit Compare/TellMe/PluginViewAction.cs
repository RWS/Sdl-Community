using System.Drawing;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.PostEdit.Versions.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction() => Name = $"{PluginResources.Plugin_Name} View";

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.PostEditVersions_Icon;
		public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.GetController<PostEditCompareViewController>().Activate();
        }
    }
}