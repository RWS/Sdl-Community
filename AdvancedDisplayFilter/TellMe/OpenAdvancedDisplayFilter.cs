using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public override void Execute()
		{
			var displayFilterController = SdlTradosStudio.Application.GetController<DisplayFilterController>();
			displayFilterController.Activate();
		}

		public override bool IsAvailable => true;

		public override string Category => "Community Advanced Display Filter results";

		public override Icon Icon => PluginResources.AdvancedDisplayFiltersIcon;
	}
}
