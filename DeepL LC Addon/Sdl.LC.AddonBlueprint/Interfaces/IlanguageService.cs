using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.DeeplAddon.Enums;
using Sdl.Community.DeeplAddon.Models;

namespace Sdl.Community.DeeplAddon.Interfaces
{
	public interface ILanguageService
	{
		Task<List<string>> GetAvailableDeeplLanguages(string apiKey, LanguageEnum languageType);
		Task<TranslationEngineResponse> GetCorrespondingEngines(string apiKey, string sourceLanguageCode, List<string> targetLanguagesCode);
		TranslationEngine GetLanguagesFromEngineId(string engineId);
	}
}
