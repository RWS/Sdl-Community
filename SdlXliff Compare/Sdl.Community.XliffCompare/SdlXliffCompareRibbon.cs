using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XliffCompare
{
    [RibbonGroup("Sdl.Community.SdlXliffCompare", Name = "SdlXliffCompare")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class SdlXliffCompareRibbon : AbstractRibbonGroup
    {
    }
    [Action("Sdl.Community.SdlXliffCompare", Name = "SdlXliffCompare", Icon = "SDLXLIFFCompare", Description = "SdlXliffCompare")]
    [ActionLayout(typeof(SdlXliffCompareRibbon), 20, DisplayType.Large)]
    class YourStudioViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            var compare = new FormMain();
            compare.ShowDialog();
        }
    }
}
