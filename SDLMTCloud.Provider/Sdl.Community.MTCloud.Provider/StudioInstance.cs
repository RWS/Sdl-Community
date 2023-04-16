using System.Windows;
using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Application = System.Windows.Application;

namespace Sdl.Community.MTCloud.Provider
{
	[ApplicationInitializer]
	public sealed class StudioInstance : IApplicationInitializer
	{
		public void Execute()
		{
			Log.Setup();
			SetApplicationShutdownMode();
		}

		public static Form GetActiveForm()
		{
			try
			{
				var allForms = System.Windows.Forms.Application.OpenForms;
				if (allForms.Count > 0)
				{
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
			}
			catch
			{
				//ignore; catch all
			}

			return null;
		}

		//public static LanguageCloudIdentityApi GetLanguageCloudIdentityApi()
		//{
		//	try
		//	{
		//		return (LanguageCloudIdentityApi)LanguageCloudIdentityApi.Instance;
		//	}
		//	catch
		//	{
		//		// ignore; catch all
		//	}

		//	return null;
		//}

		public static EditorController GetEditorController()
		{
			try
			{
				return SdlTradosStudio.Application?.GetController<EditorController>();
			}
			catch
			{
				//catch all; ignore
			}

			return null;
		}

		private static void SetApplicationShutdownMode()
		{
			try
			{
				if (Application.Current == null)
				{
					// initialize the enviornments application instance
					new Application();
				}

				if (Application.Current != null)
				{
					Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				}
			}
			catch
			{
				//ignore; catch all
			}
		}

		public static ProjectsController GetProjectsController()
		{
			try
			{
				return SdlTradosStudio.Application?.GetController<ProjectsController>();
			}
			catch
			{
				//catch all; ignore
			}

			return null;
		}
	}
}