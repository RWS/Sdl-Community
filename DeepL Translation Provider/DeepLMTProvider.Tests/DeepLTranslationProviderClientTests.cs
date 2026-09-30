using System.Linq;
using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Model;
using Xunit;

namespace DeepLMTProvider.Tests
{
    public class DeepLTranslationProviderClientTests
    {
        private const int KiB = 1024;

        private static readonly DeepLSettings Settings = new DeepLSettings(
            Formality.Default, null, TagFormat.None, SplitSentences.Default, false, null, ModelType.Quality_Optimized, null);

        private static string Text(int bytes) => new string('a', bytes);

        [Fact]
        public void BuildCacheKey_SameTextWithDifferentContext_GivesDifferentKeys()
        {
            // Act
            var withoutContext = DeepLTranslationProviderClient.BuildCacheKey("EN", "DE", "quality_optimized", null, "default", Settings, "Hello world");
            var withContext = DeepLTranslationProviderClient.BuildCacheKey("EN", "DE", "quality_optimized", null, "default", Settings, "Hello world", "It was a football match.");
            var withOtherContext = DeepLTranslationProviderClient.BuildCacheKey("EN", "DE", "quality_optimized", null, "default", Settings, "Hello world", "It was a meeting.");

            // Assert
            Assert.Equal(3, new[] { withoutContext, withContext, withOtherContext }.Distinct().Count());
        }

        [Fact]
        public void BuildCacheKey_WithoutContext_IsUnchangedSoExistingCacheEntriesStillHit()
        {
            // Act
            var key = DeepLTranslationProviderClient.BuildCacheKey("EN", "DE", "quality_optimized", null, "default", Settings, "Hello world");

            // Assert: key produced by 8.0.6.0
            Assert.Equal("b27c92549f94ba1de632a512a41e33c679c6733b5d1a9ead10b66259096815b1", key);
        }

        [Fact]
        public void BuildBatches_WithoutContext_SplitsTextsAt128KiB()
        {
            // Arrange
            var texts = new[] { Text(50 * KiB), Text(50 * KiB), Text(50 * KiB) };

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(texts).ToList();

            // Assert
            Assert.Equal(new[] { new[] { 0, 1 }, new[] { 2 } }, batches.Select(b => b.TextIndices.ToArray()));
            Assert.All(batches, b => Assert.Null(b.Context));
        }

        [Fact]
        public void BuildBatches_WithoutContext_IgnoresUnitPositions()
        {
            // Arrange: option off, but the caller still passes where each text sits among Studio's units
            var texts = new[] { "a", "b" };

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(texts, new[] { 3, 7 }, null).ToList();

            // Assert
            var batch = Assert.Single(batches);
            Assert.Equal(new[] { 0, 1 }, batch.TextIndices);
            Assert.Null(batch.Context);
        }

        [Fact]
        public void BuildBatches_WithContext_WhenBatchFits_SendsOneRequestWithEveryUnitAsContext()
        {
            // Arrange: units 0 and 3 are context only (e.g. masked out by Studio)
            var texts = new[] { "a", "b" };
            var unitPositions = new[] { 1, 2 };
            var contextUnits = new[] { "prev", "a", "b", "next" };

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(texts, unitPositions, contextUnits).ToList();

            // Assert
            var batch = Assert.Single(batches);
            Assert.Equal(new[] { 0, 1 }, batch.TextIndices);
            Assert.Equal("prev\na\nb\nnext", batch.Context);
        }

        [Fact]
        public void BuildBatches_WithContext_WhenBatchTooBig_SlicesSoEachRequestCarriesOnlyItsOwnUnitsAndFits()
        {
            // Arrange: each unit costs ~60 KiB (30 KiB text + 30 KiB context), so two fit per request
            var units = Enumerable.Range(0, 4).Select(k => new string((char)('a' + k), 30 * KiB)).ToArray();

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(units, new[] { 0, 1, 2, 3 }, units).ToList();

            // Assert
            Assert.Equal(new[] { new[] { 0, 1 }, new[] { 2, 3 } }, batches.Select(b => b.TextIndices.ToArray()));
            Assert.Equal(units[0] + "\n" + units[1], batches[0].Context);
            Assert.Equal(units[2] + "\n" + units[3], batches[1].Context);
            Assert.All(batches, b => Assert.True(
                b.TextIndices.Sum(i => units[i].Length) + b.Context.Length <= 128 * KiB));
        }

        [Fact]
        public void BuildBatches_WithContext_UnitTooBigForItsOwnContext_IsSentAloneWithoutContext()
        {
            // Arrange: 70 KiB fits DeepL's limit alone, but not twice (text + context)
            var units = new[] { Text(KiB), Text(70 * KiB), Text(KiB) };

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(units, new[] { 0, 1, 2 }, units).ToList();

            // Assert
            Assert.Equal(new[] { new[] { 0 }, new[] { 1 }, new[] { 2 } }, batches.Select(b => b.TextIndices.ToArray()));
            Assert.NotNull(batches[0].Context);
            Assert.Null(batches[1].Context);
            Assert.NotNull(batches[2].Context);
        }
    }
}
