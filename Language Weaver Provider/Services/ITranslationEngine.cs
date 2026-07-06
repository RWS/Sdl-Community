using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services.Model;

namespace LanguageWeaverProvider.Services
{
    public interface ITranslationEngine
    {
        Task<IReadOnlyList<TranslationResult>> TranslateAsync(AccessToken accessToken, PairMapping mappedPair, IReadOnlyList<SegmentSerializer> segmentSerializers);
    }
}
