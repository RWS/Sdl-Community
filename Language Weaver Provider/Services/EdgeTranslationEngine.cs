using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services.Model;

namespace LanguageWeaverProvider.Services
{
    public class EdgeTranslationEngine : ITranslationEngine
    {
        public Task<IReadOnlyList<TranslationResult>> TranslateAsync(AccessToken accessToken, PairMapping mappedPair, IReadOnlyList<SegmentSerializer> segmentSerializers)
            => EdgeService.Translate(accessToken, mappedPair, segmentSerializers);
    }
}
