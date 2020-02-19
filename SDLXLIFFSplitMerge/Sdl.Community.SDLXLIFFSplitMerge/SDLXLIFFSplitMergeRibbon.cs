using System;
using System.Diagnostics;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Utilities.SplitSDLXLIFF;

namespace Sdl.Community.SDLXLIFFSplitMerge
{
	[RibbonGroup("SDLXLIFFSplitMerge", Name = "SDLXLIFF Split / Merge")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class SDLXLIFFSplitMergeRibbon : AbstractRibbonGroup
	{
		[Action("Sdl.Community.SDLXLIFFSplitMerge", Name = "SDLXLIFF Split / Merge", Icon = "FilterDefinition_C77668_", Description = "SDLXLIFF Split / Merge")]
		[ActionLayout(typeof(SDLXLIFFSplitMergeRibbon), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 10, DisplayType.Large)]
		public class SDLXLIFFSplitMergeAction : AbstractAction
		{
			protected override void Execute()
			{
				var wizardPage = new WizardPage(true);
				wizardPage.ShowDialog();
			}
		}

		[Action("SDLXLIFFSplitMergeHelpAction", Icon = "Informative_Icon", Name = "Help", Description = "A Wiki page will be opened in the browser, containing user's documentation")]
		[ActionLayout(typeof(SDLXLIFFSplitMergeRibbon), 10, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 10, DisplayType.Large)]
		public class SDLXLIFFSplitMergeHelpAction : AbstractViewControllerAction<FilesController>
		{
			protected override void Execute()
			{
				Process.Start(new ProcessStartInfo("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3166.sdlxliff-split-merge"));
			}
		}
	}
}