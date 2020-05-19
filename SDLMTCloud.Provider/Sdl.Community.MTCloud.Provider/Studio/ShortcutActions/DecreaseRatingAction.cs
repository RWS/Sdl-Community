using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "DecreaseRatingId",
		Name = "Decrease rating",
		Description = "Decrease the rating of the translation", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class DecreaseRatingAction : AbstractAction, ISDLMTCloudAction
	{
		public override void Initialize()
		{
			base.Initialize();
			OptionName = nameof(RateItViewModel.DecreaseRating);
		}

		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.DecreaseRating();
		}

		public string OptionName { get; set; }
	}
}
