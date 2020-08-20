using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "UnintelligibillityOptionId",
		Name = "Unintelligibillity",
		Description =
			"Check/Uncheck Unintelligibillity option", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class SetUnintelligibillityAction : AbstractAction, ISDLMTCloudAction
	{
		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.SetRateOptionFromShortcuts(Text);
		}
	}
}