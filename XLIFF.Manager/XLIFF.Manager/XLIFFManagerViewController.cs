using System;
using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.Controls;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.XLIFF.Manager
{
	[View(
		Id = "XLIFFManager_View",
		Name = "XLIFFManager_Name",
		Description = "XLIFFManager_Description",
		Icon = "Icon",
		AllowViewParts = true,
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class XLIFFManagerViewController : AbstractViewController
	{
		private ProjectFilesViewModel _projectFilesViewModel;
		private ProjectsNavigationViewModel _projectsNavigationViewModel;
		private ProjectFileActivityViewModel _projectFileActivityViewModel;
		private ProjectFileActivityViewController _projectFileActivityViewController;
		private IStudioEventAggregator _eventAggregator;

		protected override void Initialize(IViewContext context)
		{
			ActivationChanged += OnActivationChanged;

			_eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			_eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>()?.Subscribe(OnStudioWindowCreatedNotificationEvent);		
		}
		
		protected override Control GetContentControl()
		{
			_projectFilesViewModel = new ProjectFilesViewModel();
			var viewControl = new ProjectFilesViewControl(_projectFilesViewModel);
			return viewControl;
		}

		protected override Control GetExplorerBarControl()
		{
			_projectsNavigationViewModel = new ProjectsNavigationViewModel();
			var viewControl = new ProjectsNavigationViewControl(_projectsNavigationViewModel);

			return viewControl;
		}

		private void OnStudioWindowCreatedNotificationEvent(StudioWindowCreatedNotificationEvent e)
		{

		}

		private void OnActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			if (e.Active)
			{
				SetProjectFileActivityViewController();
			}
		}

		private void SetProjectFileActivityViewController()
		{
			if (_projectFileActivityViewController == null)
			{
				try
				{					
					_projectFileActivityViewController = SdlTradosStudio.Application.GetController<ProjectFileActivityViewController>();

					_projectFileActivityViewModel = new ProjectFileActivityViewModel();
					_projectFileActivityViewController.ViewModel = _projectFileActivityViewModel;
				}
				catch
				{
					// catch all; unable to locate the controller
				}
			}
		}

		public override void Dispose()
		{
			_projectFilesViewModel?.Dispose();
			_projectsNavigationViewModel?.Dispose();

			base.Dispose();
		}
	}
}
