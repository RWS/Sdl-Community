using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider
{
	[ApplicationInitializer]
	public sealed class AppInitializer : IApplicationInitializer
	{		
		public void Execute()
		{
			if (Application.Current == null)
			{
				new Application();
			}

			if (Application.Current != null)
			{
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			}
		}

		public static ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application?.GetController<ProjectsController>();
		}

		public static FilesController GetFileController()
		{
			return SdlTradosStudio.Application?.GetController<FilesController>();
		}

		public static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application?.GetController<EditorController>();
		}
	}
}