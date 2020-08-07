﻿using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio.ShortcutActions
{
	[Action(Id = "GrammarOptionId",
		Name = "Grammar option",
		Description = "Check/Uncheck Grammar option", //TODO:Move this in a resource file after we confirm the exact string
		ContextByType = typeof(EditorController))]
	public class SetGrammarAction : AbstractAction, ISDLMTCloudAction
	{
		public override void Initialize()
		{
			base.Initialize();
			OptionName = nameof(RateItViewModel.GrammarOption);
		}
		protected override void Execute()
		{
			var rateItController = SdlTradosStudio.Application.GetController<RateItController>();
			rateItController?.RateIt?.SetRateOptionFromShortcuts(nameof(RateItViewModel.GrammarOption));
		}

		public string OptionName { get; set; }
	}
}
