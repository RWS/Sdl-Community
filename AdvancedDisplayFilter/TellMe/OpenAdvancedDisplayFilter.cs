using System.Drawing;
using Sdl.Community.AdvancedDisplayFilter.Controls;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.TellMe
{
	public class OpenAdvancedDisplayFilter : AbstractTellMeAction
	{
		public OpenAdvancedDisplayFilter()
		{
			Name = "Show Community Advanced Display Filter";
		}

		public override string Category => "Community Advanced Display Filter results";

		public override Icon Icon => PluginResources.AdvancedDisplayFilter_Icon;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			var displayFilterController = SdlTradosStudio.Application.GetController<DisplayFilterController>();
			displayFilterController.Activate();
		}
	}
}