using System.Drawing;
using System.Linq;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.AdvancedDisplayFilter.TellMe
{
	public class OpenProjectFilesAction : AbstractTellMeAction
	{
		public OpenProjectFilesAction()
		{
			Name = "Open current project's files in the editor";
		}

		public override string Category => "Community Advanced Display Filter results";

		public override Icon Icon => PluginResources.OpenFiles;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var projectFiles = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject
				.GetTargetLanguageFiles().ToList();

			foreach (var projectFile in projectFiles)
			{
				editorController.Open(projectFile, EditingMode.Translation);
			}
		}
	}
}