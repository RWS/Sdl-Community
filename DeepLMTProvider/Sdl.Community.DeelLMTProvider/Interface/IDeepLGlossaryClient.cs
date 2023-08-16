using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Interface
{
	public interface IDeepLGlossaryClient
	{
		Task<(bool Success, List<GlossaryInfo> Result, string FailureMessage)> GetGlossaries(string apiKey);

		Task<(bool Success, List<GlossaryLanguagePair> Result, string FailureMessage)> GetGlossarySupportedLanguagePairs(string apiKey);

		Task<(bool Success, GlossaryInfo result, string FailureMessage)> ImportGlossary(Glossary glossary, string apiKey);
	}
}