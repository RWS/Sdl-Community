using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Services.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Xunit;

namespace LanguageWeaverProviderTests
{
    public class PlainTextBatchTranslatorTests
    {
        private static readonly CultureCode TargetCulture = new CultureCode("de-DE");

        [Fact]
        public void Translate_NullSourceList_ReturnsEmpty()
        {
            var engine = new RecordingTranslationEngine(_ => Array.Empty<string>());
            var translator = CreateTranslator(engine);

            var result = translator.Translate(null, new PairMapping());

            Assert.Empty(result);
            Assert.Equal(0, engine.CallCount);
        }

        [Fact]
        public void Translate_EmptySourceList_ReturnsEmpty()
        {
            var engine = new RecordingTranslationEngine(_ => Array.Empty<string>());
            var translator = CreateTranslator(engine);

            var result = translator.Translate(Array.Empty<Segment>(), new PairMapping());

            Assert.Empty(result);
            Assert.Equal(0, engine.CallCount);
        }

        [Fact]
        public void Translate_PlainSegments_SendsSegmentTextsAndReturnsTranslations()
        {
            var s1 = new Segment(); s1.Add("Hello");
            var s2 = new Segment(); s2.Add("World");

            var engine = new RecordingTranslationEngine(inputs =>
                inputs.Select(input => input + "-translated").ToArray());

            var translator = CreateTranslator(engine);

            var results = translator.Translate(new[] { s1, s2 }, new PairMapping());

            Assert.Equal(new[] { "Hello", "World" }, engine.LastInput);
            Assert.Equal(2, results.Count);
            Assert.Equal("Hello-translated", results[0].Translation.ToPlain());
            Assert.Equal("World-translated", results[1].Translation.ToPlain());
            Assert.Equal(TargetCulture, results[0].Translation.Culture);
        }

        [Fact]
        public void Translate_TaggedSegment_SendsPlaceholderAndRestoresTag()
        {
            var tag = new Tag(TagType.Start, "1", 1);
            var source = new Segment();
            source.Add("Click ");
            source.Add(tag);
            source.Add("here");

            var engine = new RecordingTranslationEngine(inputs =>
            {
                Assert.Single(inputs);
                Assert.Equal("Click <x id=\"0\"/>here", inputs[0]);
                return new[] { "Klicke <x id=\"0\"/>hier" };
            });

            var translator = CreateTranslator(engine);

            var results = translator.Translate(new[] { source }, new PairMapping());

            var element = results[0].Translation.Elements;
            Assert.Equal(3, element.Count);
            Assert.Equal("Klicke ", ((Text)element[0]).Value);
            Assert.Same(tag, element[1]);
            Assert.Equal("hier", ((Text)element[2]).Value);
        }

        [Fact]
        public void Translate_PropagatesQualityEstimation()
        {
            var s1 = new Segment(); s1.Add("a");
            var engine = new RecordingTranslationEngine(_ => new[] { "x" }, qualityEstimations: new[] { "Good" });
            var translator = CreateTranslator(engine);

            var results = translator.Translate(new[] { s1 }, new PairMapping());

            Assert.Equal("Good", results[0].QualityEstimation);
        }

        [Fact]
        public void Translate_EngineReturnsNull_ReturnsEmpty()
        {
            var s1 = new Segment(); s1.Add("a");
            var engine = new RecordingTranslationEngine(_ => null);
            var translator = CreateTranslator(engine);

            var results = translator.Translate(new[] { s1 }, new PairMapping());

            Assert.Empty(results);
        }

        [Fact]
        public void Translate_FewerEngineResultsThanInputs_FillsMissingWithEmptyTranslations()
        {
            var s1 = new Segment(); s1.Add("a");
            var s2 = new Segment(); s2.Add("b");
            var engine = new RecordingTranslationEngine(_ => new[] { "x" });
            var translator = CreateTranslator(engine);

            var results = translator.Translate(new[] { s1, s2 }, new PairMapping());

            Assert.Equal(2, results.Count);
            Assert.Equal("x", results[0].Translation.ToPlain());
            Assert.Empty(results[1].Translation.Elements);
        }

        [Fact]
        public void Translate_UsesCurrentAccessTokenFromAccessor_OnEachCall()
        {
            var s1 = new Segment(); s1.Add("a");
            var engine = new RecordingTranslationEngine(_ => new[] { "x" });

            AccessToken token = new AccessToken { Token = "first" };
            var translator = new PlainTextBatchTranslator(engine, () => token, TargetCulture);

            translator.Translate(new[] { s1 }, new PairMapping());
            Assert.Equal("first", engine.LastAccessToken?.Token);

            token = new AccessToken { Token = "second" };
            translator.Translate(new[] { s1 }, new PairMapping());
            Assert.Equal("second", engine.LastAccessToken?.Token);
        }

        [Fact]
        public void Constructor_NullEngine_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PlainTextBatchTranslator(null, () => new AccessToken(), TargetCulture));
        }

        [Fact]
        public void Constructor_NullAccessor_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PlainTextBatchTranslator(new RecordingTranslationEngine(_ => Array.Empty<string>()), null, TargetCulture));
        }

        private static PlainTextBatchTranslator CreateTranslator(ITranslationEngine engine)
            => new PlainTextBatchTranslator(engine, () => new AccessToken(), TargetCulture);

        private sealed class RecordingTranslationEngine : ITranslationEngine
        {
            private readonly Func<string[], string[]> _translate;
            private readonly string[] _qualityEstimations;

            public RecordingTranslationEngine(Func<string[], string[]> translate, string[] qualityEstimations = null)
            {
                _translate = translate;
                _qualityEstimations = qualityEstimations;
            }

            public int CallCount { get; private set; }
            public string[] LastInput { get; private set; }
            public AccessToken LastAccessToken { get; private set; }

            public Task<IReadOnlyList<TranslationResult>> TranslateAsync(AccessToken accessToken, PairMapping mappedPair, string[] plainTextSegments)
            {
                CallCount++;
                LastAccessToken = accessToken;
                LastInput = plainTextSegments;

                var translations = _translate(plainTextSegments);
                if (translations is null)
                {
                    return Task.FromResult<IReadOnlyList<TranslationResult>>(null);
                }

                var results = translations
                    .Select((t, i) => new TranslationResult
                    {
                        Translation = t,
                        QualityEstimation = _qualityEstimations != null && i < _qualityEstimations.Length ? _qualityEstimations[i] : null
                    })
                    .ToList();

                return Task.FromResult<IReadOnlyList<TranslationResult>>(results);
            }
        }
    }
}
