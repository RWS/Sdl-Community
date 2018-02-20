using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Utilities.SplitSDLXLIFF;

namespace Sdl.Community.SDLXLIFFSplitMerge
{
	[RibbonGroup("SDLXLIFFSplitMerge", Name = "SDLXLIFF Split & Merge")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class SDLXLIFFSplitMergeRibbon : AbstractRibbonGroup
	{
		[Action("Sdl.Community.SDLXLIFFSplitMerge", Name = "SDLXLIFF Split & Merge", Icon = "SplitMerge_Icon", Description = "SDLXLIFF Split & Merge")]
		[ActionLayout(typeof(SDLXLIFFSplitMergeRibbon), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
		public class SDLXLIFFSplitMergeAction : AbstractAction
		{
			protected override void Execute()
			{
				WizardPage wizardPage = new WizardPage();
				wizardPage.ShowDialog();
			}
		}
	}
}