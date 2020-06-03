using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;

namespace WinFormControlLoading
{
	[View(
		Id = "WinFormControl",
		Name = "WinFormControl",
		Description = "Sample code for loading the control in Studio 2021",
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class ViewController : AbstractViewController
	{
		private static readonly UserControl1 UserControl1 = new UserControl1();
		private static readonly UserControl2 UserControl2 = new UserControl2();

		protected override void Initialize(IViewContext context)
		{
		}

		protected override IUIControl GetExplorerBarControl()
		{
			return UserControl2;
		}

		protected override IUIControl GetContentControl()
		{
			return UserControl1;
		}
	}
}