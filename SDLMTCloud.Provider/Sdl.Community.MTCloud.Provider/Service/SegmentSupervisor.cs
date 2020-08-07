using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class SegmentSupervisor
	{
		private readonly ITranslationService _translationService;

		public SegmentSupervisor(ITranslationService translationService)
		{
			_translationService = translationService;
			var editor = SdlTradosStudio.Application.GetController<EditorController>();
			//editor.Se
		}

		public async Task SendFeedback()
		{
			var accountId = _translationService.ConnectionService.Credential.AccountId;
			var feedBack = new FeedbackRequest();
			await _translationService.CreateTranslationFeedback(feedBack, accountId);
		}
	}
}