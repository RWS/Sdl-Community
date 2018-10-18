using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AmazonTranslateTradosPlugin
{
	[RibbonGroup("Sdl.Community.AmazonProvider", Name = "Amazon Translation Provider", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	class RibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.AmazonTranslate", Name = "Help", Icon = "question", Description = "An wiki page will be opened in browser uith user documentation")]
	[ActionLayout(typeof(RibbonGroup), 250, DisplayType.Large)]
	public class AmazonTranslateHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3315.amazon-translate-mt-provider");
		}
	}
}
