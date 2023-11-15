using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace ChromiumBrowserSupport.Ribbon
{
	internal class RibbonGroups
	{
		[RibbonGroup("ChromiumBrowserSupport_Group_Id", Name = "ChromiumBrowserSupport_Group_Name")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
		public class ConfigurationGroup : AbstractRibbonGroup
		{
		}
	}
}
