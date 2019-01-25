using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ApplyTMTemplate
{
	[RibbonGroup("ApplyTMTemplateRibbonGroup", Name = "Apply TM Template")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class ApplyTMTemplateRibbonGroup : AbstractRibbonGroup
	{
	}
}