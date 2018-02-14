using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace TQA
{
    [RibbonGroup( "TqaRibbonGroup", Name = "TQA Automation" )]
    [RibbonGroupLayout( LocationByType = typeof( TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation ) )]
    class TqaRibbonGroup : AbstractRibbonGroup
    {
    }
}
