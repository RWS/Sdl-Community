using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services.Model;

namespace LanguageWeaverProvider.Services
{
    public class CloudTranslationEngine : ITranslationEngine
    {
        public Task<IReadOnlyList<TranslationResult>> TranslateAsync(AccessToken accessToken, PairMapping mappedPair, IReadOnlyList<SegmentSerializer> segmentSerializers)
            => CloudService.Translate(accessToken, mappedPair, segmentSerializers.Select(ser => ser.SerializedSegment).ToArray());
    }
}
