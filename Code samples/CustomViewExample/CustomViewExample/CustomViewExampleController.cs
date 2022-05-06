using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CustomViewExample.Model;
using CustomViewExample.Services;
using CustomViewExample.View;
using CustomViewExample.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace CustomViewExample
{
	[View(
		Id = "CustomViewExample_View",
		Name = "CustomViewExample_Name",
		Description = "CustomViewExample_Description",
		Icon = "CustomViewExample_Icon",
		AllowViewParts = true,
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	internal class CustomViewExampleController : AbstractViewController
	{
		private ContentView _contentView;
		private NavigationView _navigationView;

		private ContentViewModel _contentViewModel;
		private NavigationViewModel _navigationViewModel;

		private ProjectsController _projectsController;
		private ImageService _imageService;
		private ProjectsService _projectsService;

		private List<CustomViewProject> _customViewProjects;
		private CancellationTokenSource _cancellationToken;

		protected override void Initialize(IViewContext context)
		{
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_imageService = new ImageService();
			_projectsService = new ProjectsService(_projectsController, _imageService);
			_customViewProjects = _projectsService.GetProjects();

			ActivationChanged += OnActivationChanged;
		}

		protected override IUIControl GetContentControl()
		{
			if (_contentView != null)
			{
				return _contentView;
			}

			_contentViewModel = new ContentViewModel(_customViewProjects.FirstOrDefault());
			_contentView = new ContentView
			{
				DataContext = _contentViewModel
			};

			return _contentView;
		}

		protected override IUIControl GetExplorerBarControl()
		{
			if (_navigationView != null)
			{
				return _navigationView;
			}

			_navigationViewModel = new NavigationViewModel(_customViewProjects);
			_navigationView = new NavigationView
			{
				DataContext = _navigationViewModel
			};

			_navigationViewModel.SelectedProjectChanged += OnSelectedProjectChanged;

			return _navigationView;
		}

		private void OnActivationChanged(object sender, ActivationChangedEventArgs args)
		{
			if (!args.Active)
			{
				return;
			}

			var studioProject = _projectsController.CurrentProject ?? _projectsController.SelectedProjects.FirstOrDefault();
			if (studioProject != null)
			{
				var customViewProject = _customViewProjects.FirstOrDefault(a => a.Id == studioProject.GetProjectInfo().Id.ToString());
				_navigationViewModel.SelectedProject = customViewProject;
			}
		}

		private void OnSelectedProjectChanged(object sender, CustomViewProject customViewProject)
		{
			_contentViewModel.Project = customViewProject;

			if (_contentViewModel.Project != null)
			{
				_cancellationToken?.Cancel();
				_cancellationToken = new CancellationTokenSource();

				System.Threading.Tasks.Task.Run(async delegate
				{
					await System.Threading.Tasks.Task.Delay(500, _cancellationToken.Token);
				}).ContinueWith(c =>
				{
					if (c.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
					{
						ActivateStudioProject(customViewProject);
					}
				});
			}
		}

		private void ActivateStudioProject(CustomViewProject customViewProject)
		{
			_projectsController.BeginInvoke(new Action(delegate
			{
				var studioProject = _projectsController.GetProjects().FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == customViewProject.Id);
				var selectedStudioProject = _projectsController.CurrentProject?.GetProjectInfo();

				if (studioProject != null && selectedStudioProject?.Id.ToString() != customViewProject.Id)
				{
					_projectsController.ActivateProject(studioProject);
					_projectsController.SelectedProjects = new[] { studioProject };
				}
			}));
		}
	}
}
