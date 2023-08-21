using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IDeepLGlossaryClient
    {
        Task<ActionResult<GlossaryInfo>> DeleteGlossary(string apiKey, string glossaryId);

        Task<ActionResult<List<GlossaryInfo>>> GetGlossaries(string apiKey);

        Task<ActionResult<List<GlossaryLanguagePair>>> GetGlossarySupportedLanguagePairs(string apiKey, bool continueOnCapturedContext = true);

        Task<ActionResult<GlossaryInfo>> ImportGlossary(Glossary glossary, string apiKey);

        Task<ActionResult<List<GlossaryEntry>>> RetrieveGlossaryEntries(string glossaryId, string apiKey);
    }
}