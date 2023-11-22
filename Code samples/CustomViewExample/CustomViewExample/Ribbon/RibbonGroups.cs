using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace CustomViewExample.Ribbon
{
	internal class RibbonGroups
	{
		[RibbonGroup("CustomViewExample_ConfigurationGroup", Name = "CustomViewExample_ConfigurationGroup_Name")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
		public class ConfigurationGroup : AbstractRibbonGroup
		{
		}

		[RibbonGroup("CustomViewExample_ActionsGroup", Name = "CustomViewExample_ActionsGroup_Name")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
		public class ActionsGroup : AbstractRibbonGroup
		{
		}

		[RibbonGroup("CustomViewExample_EditorActionsGroup", "CustomViewExample_EditorActionsGroup_Name")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorReviewRibbonTabLocation))]
		public class EditorActionsGroup : AbstractRibbonGroup
		{
		}
	}
}
