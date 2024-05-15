using System;
using System.Linq;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity
{
	[ApplicationInitializer]
	public class ApplicationContext : IApplicationInitializer
	{
		private static ProjectsController _projectsController;
		private static QualitivityViewController _qualitivityViewController;

		public static ProjectsController ProjectsController =>
			_projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

		public static QualitivityViewController QualitivityViewController => _qualitivityViewController ??= SdlTradosStudio
					.Application
			.GetController<QualitivityViewController>();

		public static FileBasedProject GetSelectedProject()
		{
			var currentProject = ProjectsController.CurrentProject;

			var projectsTotal = ProjectsController.SelectedProjects.Count();
			if (projectsTotal == 0 && currentProject != null)
				projectsTotal = 1;

			return projectsTotal is > 1 or 0
				? null
				: ProjectsController.SelectedProjects.FirstOrDefault() ?? currentProject;
		}

		public void Execute() => SdlTradosStudio.Application.GetService<IStudioEventAggregator>()
					.GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);

		private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent obj) =>
			QualitivityViewController.Initialize();
	}
}