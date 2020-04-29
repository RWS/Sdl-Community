using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    /// <summary>
    /// The ribbon group where we will put our icon
    /// </summary>
    [RibbonGroup("ApplyStudioProjectTemplateRibbonGroup", Name = "Apply Project Template")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class ApplyStudioProjectTemplateRibbonGroup : AbstractRibbonGroup
    {
    }
}
