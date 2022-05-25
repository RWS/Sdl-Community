using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Trados.Transcreate.Actions
{
	[Action("TranscreateManager_OpenFileForSignOff_Action",
		Name = "TranscreateManager_OpenFileForSignOff_Name",
		Description = "TranscreateManager_OpenFileForSignOff_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "OpenForSignOff"
	)]
	[ActionLayout(typeof(TranscreateManagerOpenGroup), 2, DisplayType.Normal)]
	public class OpenFileForSignOffAction : AbstractOpenFileInEditorAction
	{
		protected override void Execute()
		{
			OpenFile(EditingMode.SignOff);
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
