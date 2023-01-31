using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace InterpretBank.Studio
{
	public static class StudioContext
	{
		private static ProjectsController _projectsController;

		public static ProjectsController ProjectsController =>
			_projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();
	}
}