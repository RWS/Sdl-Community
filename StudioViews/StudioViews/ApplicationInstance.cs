using System.Windows;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Application = System.Windows.Application;

namespace Sdl.Community.StudioViews
{
	[ApplicationInitializer]
	internal class ApplicationInstance : IApplicationInitializer
	{
		private static FilesController _filesController;
		private static EditorController _editorController;
		private static ProjectsController _projectsController;

		public void Execute()
		{
			SetApplicationShutdownMode();
		}

		public static FilesController FilesController => _filesController ??= SdlTradosStudio.Application.GetController<FilesController>();
		public static EditorController EditorController => _editorController ??= SdlTradosStudio.Application.GetController<EditorController>();
		public static ProjectsController ProjectsController => _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

		public static Form GetActiveForm()
		{
			var allForms = System.Windows.Forms.Application.OpenForms;
			var activeForm = allForms[allForms.Count - 1];
			foreach (Form form in allForms)
			{
				if (form.GetType().Name == "StudioWindowForm")
				{
					activeForm = form;
					break;
				}
			}

			return activeForm;
		}

		private static void SetApplicationShutdownMode()
		{
			if (Application.Current == null)
			{
				// initialize the environments application instance
				new Application();
			}

			if (Application.Current != null)
			{
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			}
		}
	}
}
