using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "UnintelligenceOptionId",
		Name = "Unintelligence option",
		Description =
			"Check/Uncheck Unintelligence option", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class SetUnintelligenceAction : AbstractAction, ISDLMTCloudAction
	{
		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.SetRateOptionFromShortcuts(nameof(RateItViewModel.UnintelligenceOption));
		}

		public void LoadTooltip(string tooltipText)
		{
			ToolTipText = tooltipText;
		}
	}
}
