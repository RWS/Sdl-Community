using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services.Model;

namespace LanguageWeaverProvider.Services
{
    public class CloudTranslationEngine : ITranslationEngine
    {
        public Task<IReadOnlyList<TranslationResult>> TranslateAsync(AccessToken accessToken, PairMapping mappedPair, string[] plainTextSegments)
            => CloudService.Translate(accessToken, mappedPair, plainTextSegments);
    }
}
