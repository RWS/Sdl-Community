using System;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ProjectTerms.Plugin
{
	public static class StudioContext
	{
		private static ProjectsController _projectsController;

		public static event Action ControllersAvailableEvent;

		public static bool EventRaised { get; set; }

		public static ProjectsController ProjectsController
		{
			get
			{
				_projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

				if (_projectsController is not null) RaiseControllersAvailableEvent();
				return _projectsController;
			}
		}

		public static void RaiseControllersAvailableEvent()
		{
			if (EventRaised)
				return;

			EventRaised = true;
			ControllersAvailableEvent?.Invoke();
		}
	}
}