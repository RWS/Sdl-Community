using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sdl.LC.AddonBlueprint.Enums;
using Sdl.LC.AddonBlueprint.Models;

namespace Sdl.LC.AddonBlueprint.Interfaces
{
	public interface ITranslationService
	{
		Task<List<string>> GetAvailableDeeplLanguages(string apiKey, LanguageEnum languageType);

		Task<List<TranslationEngineResponse>> GetCorrespondingEngines(string apiKey, string sourceLanguageCode, List<string> targetLanguagesCode);
	}
}
