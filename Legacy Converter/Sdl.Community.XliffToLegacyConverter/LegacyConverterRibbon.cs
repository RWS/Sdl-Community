using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XliffToLegacyConverter
{
    [RibbonGroup("Sdl.Community.LegacyConverter", Name = "LegacyConverter")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class LegacyConverterRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.LegacyConverter", Name = "LegacyConverter", Icon = "SdlXliffToLegacyConverter", Description = "LegacyConverter")]
    [ActionLayout(typeof(LegacyConverterRibbon), 20, DisplayType.Large)]
    public class LegacyConverterViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            var legacyConverter = new FormMain();
            legacyConverter.ShowDialog();
        }
    }
}
