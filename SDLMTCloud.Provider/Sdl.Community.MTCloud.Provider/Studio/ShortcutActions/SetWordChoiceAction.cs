using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "WordChoiceOptionId",
		Name = "Word Choice option",
		Description =
			"Check/Uncheck Word Choice option", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class SetWordChoiceAction : AbstractAction, ISDLMTCloudAction
	{
		public override void Initialize()
		{
			base.Initialize();
			OptionName = nameof(RateItViewModel.WordChoiceOption);
		}
		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.SetRateOptionFromShortcuts(nameof(RateItViewModel.WordChoiceOption));
		}

		public string OptionName { get; set; }
	}
}
