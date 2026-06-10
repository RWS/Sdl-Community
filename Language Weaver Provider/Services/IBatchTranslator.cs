using System.Collections.Generic;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.XliffConverter.Model;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.Services
{
    /// <summary>
    /// Translates a batch of SDL <see cref="Segment"/>s and returns one
    /// <see cref="EvaluatedSegment"/> per input segment, preserving inline tags
    /// across the translation round-trip. Implementations encapsulate the
    /// concrete tag-preservation strategy and the call to the underlying
    /// translation engine.
    /// </summary>
    public interface IBatchTranslator
    {
        IReadOnlyList<EvaluatedSegment> Translate(IReadOnlyList<Segment> sourceSegments, PairMapping mappedPair);
    }
}
