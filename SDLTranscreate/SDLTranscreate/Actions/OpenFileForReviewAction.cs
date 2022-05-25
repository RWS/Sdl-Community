using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Trados.Transcreate.Actions
{
	[Action("TranscreateManager_OpenFileForReview_Action",
		Name = "TranscreateManager_OpenFileForReview_Name",
		Description = "TranscreateManager_OpenFileForReview_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "OpenForReview"
	)]
	[ActionLayout(typeof(TranscreateManagerOpenGroup), 3, DisplayType.Normal)]
	public class OpenFileForReviewAction : AbstractOpenFileInEditorAction
	{
		protected override void Execute()
		{
			OpenFile(EditingMode.Review);
		}

		public void OpenFile()
		{
			Execute();
		}

		public override void Initialize()
		{
			Setup();
		}
	}
}
