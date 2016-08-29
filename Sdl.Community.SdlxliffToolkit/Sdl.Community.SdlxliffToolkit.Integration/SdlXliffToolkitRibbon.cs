using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.SdlxliffToolkit.Integration
{
    [RibbonGroup("Sdl.Community.SdlXliffToolkit", Name = "SdlXliff Toolkit")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class SdlXliffToolkitRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.SdlXliffToolkit", Name = "SdlXliff Toolkit", Icon = "toolkit- 128", Description = "SdlXliff Toolkit")]
    [ActionLayout(typeof(SdlXliffToolkitRibbon), 20, DisplayType.Large)]
    class SdlXliffToolkitViewPart:AbstractAction
    {
        protected override void Execute()
        {
            var toolkit = new SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange();
            toolkit.ShowDialog();
        }
    }
}
