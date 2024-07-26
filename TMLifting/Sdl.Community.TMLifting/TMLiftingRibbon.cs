using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Windows.Forms;
using System.Reflection;
using Sdl.Community.TMLifting.Helpers;
using System.Diagnostics;
using Sdl.Desktop.IntegrationApi.Interfaces;


namespace Sdl.Community.TMLifting
{
	[RibbonGroup("ConnectorViewRibbonGroup", "Wiki")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	class TMLiftingRibbonGroup : AbstractRibbonGroup
	{
	}

	[View(
        Id = "TMLifting",
        Name = "TM Lifting",
        Description = "",
        Icon = "icon",
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
    public class TMLiftingRibbon : AbstractViewController
    {
        private readonly Lazy<TMLiftingForm> _viewContent = new Lazy<TMLiftingForm>();
        protected override IUIControl GetContentControl()
        {
            return _viewContent.Value;
        }
        protected override void Initialize(IViewContext context)
        {
			
		}
	}
	//[Action("HelpAction", typeof(TMLiftingRibbon), Name = "Help", Description = "An wiki page will be opened in browser with user guide", Icon = "question")]
	//[ActionLayout(typeof(TMLiftingRibbonGroup), 0, DisplayType.Large)]
	//public class HelpAction : AbstractViewControllerAction<TMLiftingRibbon>
	//{
	//	protected override void Execute()
	//	{
	//		Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3268.tm-lifting-reindex-translation-memories");
	//	}
	//}
}