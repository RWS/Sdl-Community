using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "WordChoiceOptionId",
		Name = "Word Choice",
		Description =
			"Check/Uncheck Word Choice option", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class SetWordChoiceAction : AbstractAction, ISDLMTCloudAction
	{
		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.SetRateOptionFromShortcuts(Text);
		}
	}
}