using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace InterpretBank.Studio
{
	public static class StudioContext
	{
		private static ProjectsController _projectsController;
		private static EditorController _editorController;

		public static ProjectsController ProjectsController =>
			_projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();
		
		public static EditorController EditorController =>
			_editorController ??= SdlTradosStudio.Application?.GetController<EditorController>();
	}
}