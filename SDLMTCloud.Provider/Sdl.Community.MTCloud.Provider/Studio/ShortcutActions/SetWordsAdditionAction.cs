using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "WordsAdditionOptionId",
		Name = "Words Addition option",
		Description =
			"Check/Uncheck Words Addition option", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class SetWordsAdditionAction : AbstractAction, ISDLMTCloudAction
	{
		public override void Initialize()
		{
			base.Initialize();
			OptionName = nameof(RateItViewModel.WordsAdditionOption);
		}
		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.SetRateOptionFromShortcuts(nameof(RateItViewModel.WordsAdditionOption));
		}

		public string OptionName { get; set; }
	}
}
