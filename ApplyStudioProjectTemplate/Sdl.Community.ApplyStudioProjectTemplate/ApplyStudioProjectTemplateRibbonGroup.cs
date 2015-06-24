using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    /// <summary>
    /// The ribbon group where we will put our icon
    /// </summary>
    [RibbonGroup("ApplyStudioProjectTemplateRibbonGroup", Name = "Apply Project Template")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class ApplyStudioProjectTemplateRibbonGroup : AbstractRibbonGroup
    {
    }
}
