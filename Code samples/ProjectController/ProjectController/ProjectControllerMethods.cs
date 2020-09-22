using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace ProjectController
{
	[ApplicationInitializer]
	public class ProjectControllerMethods : IApplicationInitializer
	{
		public void Execute()
		{
			//files controller
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();
			var activeProjectFromFiles = filesController?.CurrentProject;
		}
	}


	[RibbonGroup("Test", Name = "Test project icon")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class ProjectTemplateRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("TestProjIconAction", Icon = "", Name = "Subscribe to content changed event",
		Description = "")]
	[ActionLayout(typeof(ProjectTemplateRibbonGroup), 10, DisplayType.Large)]
	public class ProjectAction : AbstractAction
	{
		protected override void Execute()
		{

			//var file = @"filepath";
			//var converter = DefaultFileTypeManager.CreateInstance(true)
			//	.GetConverterToDefaultBilingual(file, file, null);
			//converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new FileProcessor()));
			//converter.Parse();
			////projects controler
			//var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			//var activeProject = projectsController?.CurrentProject;


			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var doc = editorController?.ActiveDocument;
			//Delete using editor controller available in SR1

			//doc.DeleteAllCommentsWithoutNotification();


			if (doc != null)
			{
				doc.ContentChanged += Doc_ContentChanged;
				MessageBox.Show("Subscribed to Content Changed Event", string.Empty, MessageBoxButtons.OK);
			}

		}

		private void Doc_ContentChanged(object sender, DocumentContentEventArgs e)
		{
			foreach (var segment in e.Segments)
			{
				MessageBox.Show($"fired for seg id:{segment.Properties.Id}", string.Empty, MessageBoxButtons.OK);
			}
		}
	}
}
