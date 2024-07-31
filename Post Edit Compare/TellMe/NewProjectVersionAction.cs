using System.Drawing;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.PostEdit.Versions.TellMe
{
	public class NewProjectVersionAction : AbstractTellMeAction
	{
		public NewProjectVersionAction() => Name = $"{PluginResources.Plugin_Name} New Project Version";

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.CreateProjectVersion_Icon;
		public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.GetController<PostEditCompareViewController>().Activate();
            SdlTradosStudio.Application.GetAction<CreateProjectVersionMainProject>().Run();
        }
    }
}