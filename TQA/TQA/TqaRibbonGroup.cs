using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;


namespace TQA
{
    [RibbonGroup( "TqaRibbonGroup", Name = "TQA Automation" )]
    [RibbonGroupLayout( LocationByType = typeof( StudioDefaultRibbonTabs.AddinsRibbonTabLocation ) )]
    public class TqaRibbonGroup : AbstractRibbonGroup
    {
    }
}
