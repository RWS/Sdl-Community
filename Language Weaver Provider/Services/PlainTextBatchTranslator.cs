using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.XliffConverter.Model;
using Sdl.Core.Globalization;
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
        Func<AccessToken> accessTokenAccessor,
        CultureCode targetCulture)
        : IBatchTranslator
    {
        private readonly Func<AccessToken> _accessTokenAccessor = accessTokenAccessor ?? throw new ArgumentNullException(nameof(accessTokenAccessor));
        private readonly ITranslationEngine _translationEngine = translationEngine ?? throw new ArgumentNullException(nameof(translationEngine));

        public IReadOnlyList<EvaluatedSegment> Translate(IReadOnlyList<Segment> sourceSegments, PairMapping mappedPair)
        {
            if (sourceSegments is null || sourceSegments.Count == 0) return [];

            var placers = sourceSegments.Select(s => new SegmentTagPlacer(s)).ToArray();
            var plainTexts = placers.Select(p => p.PreparedText).ToArray();

            var translationResults = _translationEngine
                .TranslateAsync(_accessTokenAccessor(), mappedPair, plainTexts)
                .Result;

            if (translationResults is null) return [];

            var evaluated = new EvaluatedSegment[placers.Length];
            for (var i = 0; i < placers.Length; i++)
            {
                var translation = i < translationResults.Count ? translationResults[i] : null;
                evaluated[i] = ToEvaluatedSegment(translation, placers[i]);
            }

            return evaluated;
        }

        private EvaluatedSegment ToEvaluatedSegment(TranslationResult result, SegmentTagPlacer placer)
        {
            var translatedText = result?.Translation ?? string.Empty;
            var targetSegment = placer.BuildTargetSegment(translatedText, targetCulture);
            return new EvaluatedSegment
            {
                Translation = targetSegment,
                QualityEstimation = result?.QualityEstimation
            };
        }
    }
}