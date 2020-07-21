using Sdl.Community.Transcreate.Common;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Transcreate.Actions
{
	[Action("TranscreateManager_CreateBackTranslationProject_Action",
		Name = "TranscreateManager_CreateBackTranslationProject_Name",
		Description = "TranscreateManager_CreateBackTranslationProject_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "Icon"
		)]
	[ActionLayout(typeof(TranscreateManagerActionsGroup), 3, DisplayType.Large)]
	public class CreateBackTranslationProjectAction : AbstractViewControllerAction<TranscreateViewController>
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
