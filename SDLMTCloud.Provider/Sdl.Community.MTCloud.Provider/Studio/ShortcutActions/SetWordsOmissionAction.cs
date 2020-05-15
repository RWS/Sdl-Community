using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "WordsOmissionId",
		Name = "Words Omission option",
		Description = "Check/Uncheck Words omission option", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class SetWordsOmissionAction: AbstractAction,ISdlMTCloudAction
	{
		public SetWordsOmissionAction()
		{
			
		}
		public override void Initialize()
		{
			//base.Initialize();
			//var shortcutService = new ShortcutService();
			//var tooltip = shortcutService.GetShotcutDetails(Id);
			//_rateItController?.RateIt?.SetOptionTooltip(nameof(RateItViewModel.WordsOmissionOption),tooltip);
		}

		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();

		rateItController?.RateIt?.SetRateOptionFromShortcuts(nameof(RateItViewModel.WordsOmissionOption));
		}

		public void LoadTooltip(string tooltip)
		{
				
		}
	}
}
