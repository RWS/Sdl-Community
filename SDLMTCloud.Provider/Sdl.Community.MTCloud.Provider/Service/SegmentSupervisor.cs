using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class SegmentSupervisor
	{
		private readonly ITranslationService _translationService;
		private EditorController _editorController;

		public SegmentSupervisor(ITranslationService translationService)
		{
			_translationService = translationService;
			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
			_editorController.ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
		}

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, System.EventArgs e)
		{
			//throw new System.NotImplementedException();
		}

		public async Task SendFeedback()
		{
			//if  (_editorController.ActiveDocument.ActiveSegmentPair.Properties.ConfirmationLevel)
			//var accountId = _translationService.ConnectionService.Credential.AccountId;
			//var feedBack = new FeedbackRequest();
			//await _translationService.CreateTranslationFeedback(feedBack, accountId);
		}


	}
}