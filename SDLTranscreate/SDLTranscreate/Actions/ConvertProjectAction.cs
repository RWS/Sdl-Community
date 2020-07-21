using Sdl.Community.Transcreate.Common;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Transcreate.Actions
{
	[Action("TranscreateManager_ConvertProject_Action", 
		Name = "TranscreateManager_ConvertProject_Name",
		Description = "TranscreateManager_ConvertProject_Description",
		ContextByType = typeof(ProjectsController),
		Icon = "Icon"
		)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 5, DisplayType.Default, "", true)]
	public class ConvertProjectAction : AbstractAction
	{
		private PathInfo _pathInfo;

		protected override void Execute()
		{

		}
	

		public override void Initialize()
		{
			Enabled = false;
			_pathInfo = new PathInfo();
		}
	}
}
