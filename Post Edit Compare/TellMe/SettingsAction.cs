using System.Drawing;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.PostEdit.Versions.TellMe
{
	public class PluginViewAction : AbstractTellMeAction
	{
		public PluginViewAction() => Name = $"{PluginResources.Plugin_Name} Settings";

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.Settings2;
		public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.GetController<PostEditCompareViewController>().Activate();
            SdlTradosStudio.Application.GetAction<PostEditCompareActionsConfiguration>().Run();
        }
    }
}