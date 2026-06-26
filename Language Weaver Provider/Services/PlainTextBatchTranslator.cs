using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services.Model;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageWeaverProvider.Services
{
    /// <summary>
    /// Default <see cref="IBatchTranslator"/> implementation. Encodes each source
    /// segment into a plain-text representation with placeholder tokens for inline
    /// tags via <see cref="SegmentTagPlacer"/>, dispatches the batch through an
    /// <see cref="ITranslationEngine"/>, and rehydrates the returned strings into
    /// SDL <see cref="Segment"/>s with their original tags placed back.
    /// </summary>
    public sealed class PlainTextBatchTranslator(
        ITranslationEngine translationEngine,
        Func<AccessToken> accessTokenAccessor)
        : IBatchTranslator
    {
        private readonly Func<AccessToken> _accessTokenAccessor = accessTokenAccessor ?? throw new ArgumentNullException(nameof(accessTokenAccessor));
        private readonly ITranslationEngine _translationEngine = translationEngine ?? throw new ArgumentNullException(nameof(translationEngine));

        public IReadOnlyList<EvaluatedSegment> Translate(IReadOnlyList<Segment> sourceSegments, PairMapping mappedPair)
        {
            if (sourceSegments is null || sourceSegments.Count == 0) return [];

            var segmentSerializers = sourceSegments
                .Select(sourceSegment => new SegmentSerializer(sourceSegment, mappedPair.SourceCode, mappedPair.TargetCode))
                .ToArray();

            var translationResults = _translationEngine
                .TranslateAsync(_accessTokenAccessor(), mappedPair, segmentSerializers)
                .Result;

            if (translationResults is null) return [];

            var evaluated = new EvaluatedSegment[segmentSerializers.Length];
            for (var i = 0; i < segmentSerializers.Length; i++)
            {
                var translation = i < translationResults.Count ? translationResults[i] : null;
                evaluated[i] = ToEvaluatedSegment(translation, segmentSerializers[i]);
            }

            return evaluated;
        }

        private EvaluatedSegment ToEvaluatedSegment(TranslationResult result, SegmentSerializer segmentSerializer)
        {
            var translatedText = result?.Translation ?? string.Empty;
            var targetSegment = segmentSerializer.DeserializeSegment(translatedText);
            return new EvaluatedSegment
            {
                Translation = targetSegment,
                QualityEstimation = result?.QualityEstimation
            };
        }
    }
}