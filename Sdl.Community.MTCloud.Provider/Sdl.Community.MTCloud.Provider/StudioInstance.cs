using System.Windows;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguageCloud.IdentityApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Application = System.Windows.Application;

namespace Sdl.Community.MTCloud.Provider
{
	[ApplicationInitializer]
	public sealed class StudioInstance : IApplicationInitializer
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

			GetLanguageCloudIdentityApi = LanguageCloudIdentityApi.Instance;			
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


		public static LanguageCloudIdentityApi GetLanguageCloudIdentityApi { get; set; }
	}
}