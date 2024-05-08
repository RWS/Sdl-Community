using System.Reflection;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace Sdl.Community.XliffCompare
{
	public static class ApplicationContext
	{
		public static SDLXLIFFViewPartController Controller { get; set; }

		public static void CompareAction()
		{
			var activeView = GetCurrentView();

			switch (activeView)
			{
				case TradosView.ProjectsView:
					Controller = ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlXliffCompareProjectsViewPart>();
					break;

				case TradosView.FilesView:
					Controller = ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlXliffCompareFilesViewPart>();
					break;

				case TradosView.EditorView:
					Controller = ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlXliffCompareEditorViewPart>();
					break;
			}

			Controller.Show();
		}

		private static TradosView GetCurrentView()
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();

			var propertyInfo = projectController.GetType().BaseType.GetField("_studioWindow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
			var studioWindow = propertyInfo.GetValue(projectController);

			var propertyInfo2 = studioWindow.GetType()
				.GetProperty("ActiveView", BindingFlags.Instance | BindingFlags.Public);

			var activeView = propertyInfo2.GetValue(studioWindow);

			return activeView.ToString().Contains("ProjectsView") ? TradosView.ProjectsView :
				activeView.ToString().Contains("FilesView") ? TradosView.FilesView : TradosView.EditorView;
		}
	}
}