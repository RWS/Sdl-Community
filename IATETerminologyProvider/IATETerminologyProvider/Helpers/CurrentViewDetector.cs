using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.IATETerminologyProvider.Helpers
{
	public class CurrentViewDetector
	{
		public CurrentViewDetector()
		{
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();

			projectsController.ActivationChanged += ProjectsController_ActivationChanged;
			editorController.ActivationChanged += EditorController_ActivationChanged;
			filesController.ActivationChanged += FilesController_ActivationChanged;
		}

		public enum CurrentView
		{
			ProjectsView,
			FilesView,
			EditorView
		}

		public CurrentView View { get; set; }

		private void EditorController_ActivationChanged(object sender, Desktop.IntegrationApi.ActivationChangedEventArgs e)
		{
			View = CurrentView.EditorView;
		}

		private void FilesController_ActivationChanged(object sender, Desktop.IntegrationApi.ActivationChangedEventArgs e)
		{
			View = CurrentView.FilesView;
		}

		private void ProjectsController_ActivationChanged(object sender, Desktop.IntegrationApi.ActivationChangedEventArgs e)
		{
			View = CurrentView.ProjectsView;
		}
	}
}