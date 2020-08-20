using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Studio.ShortcutActions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.View
{
	public class ActionProvider : IActionProvider
	{
		public List<ISDLMTCloudAction> GetActions()
		{
			if (SdlTradosStudio.Application != null)
			{
				return new List<ISDLMTCloudAction>
				{
					SdlTradosStudio.Application?.GetAction<DecreaseRatingAction>(),
					SdlTradosStudio.Application?.GetAction<IncreaseRatingAction>(),
					SdlTradosStudio.Application?.GetAction<SetCapitalizationAction>(),
					SdlTradosStudio.Application?.GetAction<SetGrammarAction>(),
					SdlTradosStudio.Application?.GetAction<SetSpellingAction>(),
					SdlTradosStudio.Application?.GetAction<SetUnintelligibillityAction>(),
					SdlTradosStudio.Application?.GetAction<SetWordChoiceAction>(),
					SdlTradosStudio.Application?.GetAction<SetWordsAdditionAction>(),
					SdlTradosStudio.Application?.GetAction<SetWordsOmissionAction>()
				};
			}
			return new List<ISDLMTCloudAction>();
		}
	}
}