using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Transcreate.Actions
{
	[Action("TranscreateManager_CreateReport_Action",
		Name = "TranscreateManager_CreateReport_Name",
		Description = "TranscreateManager_CreateReport_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "Report"
		)]
	[ActionLayout(typeof(TranscreateManagerActionsGroup), 2, DisplayType.Large)]
	public class CreateReport : AbstractViewControllerAction<TranscreateViewController>
	{
		
		protected override void Execute()
		{
			
			//TODO
		}


	}
}
