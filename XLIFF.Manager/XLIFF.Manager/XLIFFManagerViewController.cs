using System;
using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.Controls;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.XLIFF.Manager
{
	[View(
		Id = "XLIFF.Manager.View",
		Name = "XLIFFManager_Name",
		Description = "XLIFFManager_Description",
		Icon = "icon",
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class XLIFFManagerViewController: AbstractViewController
	{
		private ProjectFilesViewModel _projectFilesViewModel;
		private ProjectsNavigationViewModel _projectsNavigationViewModel;

		protected override void Initialize(IViewContext context)
		{		
			// TODO	
		}

		protected override Control GetContentControl()
		{
			_projectFilesViewModel = new ProjectFilesViewModel();
			var viewControl = new Lazy<ProjectFilesViewControl>(() => new ProjectFilesViewControl(_projectFilesViewModel));

			return viewControl.Value;
		}

		protected override Control GetExplorerBarControl()
		{
			_projectsNavigationViewModel = new ProjectsNavigationViewModel();
			var viewControl = new Lazy<ProjectsNavigationViewControl>(() => new ProjectsNavigationViewControl(_projectsNavigationViewModel));

			return viewControl.Value;
		}		
	}
}
