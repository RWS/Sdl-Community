using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Interface
{
	public interface IDeepLGlossaryClient
	{
        Task<(bool Success, object Result, string FailureMessage)> DeleteGlossary(string apiKey, string glossaryId);
        Task<ActionResult<List<GlossaryInfo>>> GetGlossaries(string apiKey);

        Task<ActionResult<List<GlossaryLanguagePair>>> GetGlossarySupportedLanguagePairs(string apiKey);

        Task<ActionResult<GlossaryInfo>> ImportGlossary(Glossary glossary, string apiKey);
	}
}