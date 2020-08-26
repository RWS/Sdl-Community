using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "UnintelligibilityOptionId",
		Name = "Unintelligibility",
		Description =
			"Check/Uncheck Unintelligibility option", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class SetUnintelligibilityAction : AbstractAction, ISDLMTCloudAction
	{
		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.SetRateOptionFromShortcuts(Text);
		}
	}
}