using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace ProjectCredentialsTest
{
	[Action("ProjectCredentialsTest")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			var form = new Form1();
			var dialog =form.ShowDialog();
			if (dialog == DialogResult.OK)
			{
				var tmUri = new Uri(form.Url);
				var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
				projectController.CurrentProject?.Credentials?.AddCredential(tmUri, true, "", "");
			}
		}
	}
	//[Action("Merge Project files")]
	//[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	//public class MyCustomTradosStudio : AbstractAction
	//{
	//	protected override void Execute()
	//	{
	//		var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
	//		var selectedProject = projectController?.CurrentProject;
	//		if (selectedProject != null)
	//		{
	//			var sourceFiles = selectedProject.GetSourceLanguageFiles();
	//			selectedProject.SetFileRole(sourceFiles.GetIds(), FileRole.Translatable);
	//			selectedProject.CreateMergedProjectFile("MergedFile.sdlxliff", string.Empty, sourceFiles.GetIds());
	//		}
	//	}
	//}
}