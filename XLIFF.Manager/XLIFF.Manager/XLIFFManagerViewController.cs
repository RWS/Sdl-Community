using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.Controls;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.TestData;
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
		private readonly object _lockObject = new object();
		private List<ProjectModel> _projectModels;
		private ProjectFilesViewModel _projectFilesViewModel;
		private ProjectsNavigationViewModel _projectsNavigationViewModel;
		private ProjectFilesViewControl _projectFilesViewControl;
		private ProjectsNavigationViewControl _projectsNavigationViewControl;
		private ProjectFileActivityViewController _projectFileActivityViewController;
		private IStudioEventAggregator _eventAggregator;		

		protected override void Initialize(IViewContext context)
		{
			ActivationChanged += OnActivationChanged;

			_eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			_eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>()?.Subscribe(OnStudioWindowCreatedNotificationEvent);

			// TODO this will be replaced with a call to recover the relevant data from the projects loaded in Studio
			var testDataUtil = new TestDataUtil();
			_projectModels = testDataUtil.GetTestProjectData();
		}

		protected override Control GetContentControl()
		{
			if (_projectFilesViewControl == null)
			{
				_projectFilesViewModel = new ProjectFilesViewModel(_projectModels?.Count > 0 ? _projectModels[0].ProjectFileActionModels : null);
				_projectFilesViewControl = new ProjectFilesViewControl(_projectFilesViewModel);

				_projectsNavigationViewModel.ProjectFilesViewModel = _projectFilesViewModel;
			}

			return _projectFilesViewControl;
		}

		protected override Control GetExplorerBarControl()
		{
			if (_projectsNavigationViewControl == null)
			{
				_projectsNavigationViewModel = new ProjectsNavigationViewModel(_projectModels);
				_projectsNavigationViewControl = new ProjectsNavigationViewControl(_projectsNavigationViewModel);
			}

			return _projectsNavigationViewControl;
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
			lock (_lockObject)
			{
				if (_projectFileActivityViewController != null)
				{
					return;
				}

				try
				{
					_projectFileActivityViewController =
						SdlTradosStudio.Application.GetController<ProjectFileActivityViewController>();

					_projectFilesViewModel.ProjectFileActivityViewModel =
						new ProjectFileActivityViewModel(_projectFilesViewModel?.SelectedProjectFileAction?.ProjectFileActivityModels);

					_projectFileActivityViewController.ViewModel = _projectFilesViewModel.ProjectFileActivityViewModel;
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
