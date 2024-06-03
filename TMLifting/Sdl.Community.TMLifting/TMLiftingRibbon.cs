using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Windows.Forms;


namespace Sdl.Community.TMLifting
{
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
}