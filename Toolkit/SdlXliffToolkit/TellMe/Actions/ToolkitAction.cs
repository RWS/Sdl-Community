using System.Drawing;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace SdlXliffToolkit.TellMe.Actions
{
	public class ToolkitAction : ToolkitAbastractTellMeAction
	{
		public ToolkitAction()
		{
			Name = $"{PluginResources.Plugin_Name}";
		}

		public override Icon Icon => PluginResources.toolkit__128;

		public override void Execute()
		{
			var activeView = ApplicationInitializer.GetCurrentView();

			switch (activeView)
			{
				case TradosView.ProjectsView:
					ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitProjectsViewPart>().Show();
					break;

				case TradosView.FilesView:
					ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitFilesViewPart>().Show();
					break;

				case TradosView.EditorView:
					ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitEditorViewPart>().Show();
					break;
			}
		}
	}
}