using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace InterpretBank.Studio.Actions
{
    [RibbonGroup("InterpretBankRibbonGroup", Name = "InterpretBank")]
    [RibbonGroupLayout(LocationByType = typeof(Sdl.Desktop.IntegrationApi.DefaultLocations.StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class InterpretBankRibbonGroup : AbstractRibbonGroup
    {
    }
}