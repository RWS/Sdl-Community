using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "FocusFeedbackId",
		Name = "Focus feedback",
		Description = "Focus on the feedback area of the Rate Translations form",
		ContextByType = typeof(EditorController))]
	public class FocusFeedbackAction : AbstractAction, ISDLMTCloudAction
	{
		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.FocusFeedbackTextBox();
		}
	}
}