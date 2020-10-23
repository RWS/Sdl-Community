using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.LC.AddonBlueprint.Enums;
using Sdl.LC.AddonBlueprint.Models;

namespace Sdl.LC.AddonBlueprint.Interfaces
{
	public interface ILanguageService
	{
		Task<List<string>> GetAvailableDeeplLanguages(string apiKey, LanguageEnum languageType);
		Task<TranslationEngineResponse> GetCorrespondingEngines(string apiKey, string sourceLanguageCode, List<string> targetLanguagesCode);
		TranslationEngine GetLanguagesFromEngineId(string engineId);
	}
}
