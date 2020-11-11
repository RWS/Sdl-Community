using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "IncreaseRatingId",
		Name = "Increase rating",
		Description = "Increase the rating of the translation", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class IncreaseRatingAction : AbstractAction, ISDLMTCloudAction
	{
		protected override void Execute()
		{
			if (!Enabled) return;
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.IncreaseRating();
		}
	}
}