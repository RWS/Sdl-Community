using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task SendAllAsync_ReturnsResultsInRequestOrder_WhenSendsFinishOutOfOrder()
        {
            // Arrange: earlier requests take longer, so they finish last
            var requests = new[] { 40, 30, 20, 10, 0 };

            // Act
            var results = await DeepLTranslationProviderClient.SendAllAsync(requests,
                async delayMs => { await Task.Delay(delayMs); return delayMs; }, maxInFlight: 8);

            // Assert
            Assert.Equal(requests, results);
        }

        [Fact]
        public async Task SendAllAsync_NeverHasMoreThanMaxInFlightRequestsRunning()
        {
            // Arrange
            var inFlight = 0;
            var peak = 0;

            // Act
            await DeepLTranslationProviderClient.SendAllAsync(Enumerable.Range(0, 20).ToArray(), async _ =>
            {
                var now = Interlocked.Increment(ref inFlight);
                lock (this) peak = Math.Max(peak, now);
                await Task.Delay(20);
                Interlocked.Decrement(ref inFlight);
                return 0;
            }, maxInFlight: 3);

            // Assert
            Assert.Equal(3, peak);
        }

        private static Func<Task<HttpResponseMessage>> Responses(params int[] statusCodes)
        {
            var queue = new System.Collections.Generic.Queue<int>(statusCodes);
            return () => Task.FromResult(new HttpResponseMessage((HttpStatusCode)queue.Dequeue()));
        }

        [Fact]
        public async Task SendWithRetryAsync_RetriesTooManyRequests_ThenReturnsTheSuccess()
        {
            // Arrange
            var delays = new System.Collections.Generic.List<TimeSpan>();

            // Act
            var response = await DeepLTranslationProviderClient.SendWithRetryAsync(
                Responses(429, 200), d => { delays.Add(d); return Task.CompletedTask; });

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(delays);
        }

        [Fact]
        public async Task SendWithRetryAsync_GivesUpAfterThreeAttempts_WithGrowingBackoff()
        {
            // Arrange
            var delays = new System.Collections.Generic.List<TimeSpan>();

            // Act
            var response = await DeepLTranslationProviderClient.SendWithRetryAsync(
                Responses(429, 529, 503, 200), d => { delays.Add(d); return Task.CompletedTask; });

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            Assert.Equal(2, delays.Count);
            Assert.True(delays[1] > delays[0]);
        }

        [Fact]
        public async Task SendWithRetryAsync_WaitsForRetryAfter_WhenDeepLSendsIt()
        {
            // Arrange
            var delays = new System.Collections.Generic.List<TimeSpan>();
            var first = true;
            Func<Task<HttpResponseMessage>> send = () =>
            {
                if (!first) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                first = false;
                var tooMany = new HttpResponseMessage((HttpStatusCode)429);
                tooMany.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(7));
                return Task.FromResult(tooMany);
            };

            // Act
            await DeepLTranslationProviderClient.SendWithRetryAsync(send, d => { delays.Add(d); return Task.CompletedTask; });

            // Assert
            Assert.Equal(new[] { TimeSpan.FromSeconds(7) }, delays);
        }

        [Theory]
        [InlineData(456)] // quota exceeded: retrying cannot succeed
        [InlineData(403)]
        [InlineData(400)]
        public async Task SendWithRetryAsync_DoesNotRetryClientErrors(int status)
        {
            // Arrange
            var delays = new System.Collections.Generic.List<TimeSpan>();

            // Act
            var response = await DeepLTranslationProviderClient.SendWithRetryAsync(
                Responses(status, 200), d => { delays.Add(d); return Task.CompletedTask; });

            // Assert
            Assert.Equal(status, (int)response.StatusCode);
            Assert.Empty(delays);
        }

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
        public void BuildBatches_WithWindowAndCustomContext_PutsTheCustomContextBeforeTheWindow()
        {
            // Arrange
            var units = new[] { "a", "b" };

            // Act
            var batch = Assert.Single(DeepLTranslationProviderClient.BuildBatches(units, new[] { 0, 1 }, units, "Maria is a woman."));

            // Assert
            Assert.Equal("Maria is a woman.\na\nb", batch.Context);
        }

        [Fact]
        public void BuildBatches_WithWindowAndCustomContext_CustomContextCountsTowardTheRequestLimit()
        {
            // Arrange: two 30 KiB units fit one request (120 KiB of text + context), but not with a 10 KiB custom context
            var units = new[] { Text(30 * KiB), Text(30 * KiB) };

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(units, new[] { 0, 1 }, units, Text(10 * KiB)).ToList();

            // Assert
            Assert.Equal(2, batches.Count);
            Assert.All(batches, b => Assert.True(
                b.TextIndices.Sum(i => units[i].Length) + b.Context.Length <= 128 * KiB));
        }

        [Fact]
        public void BuildBatches_CustomContextOnly_IsTheContextOfOneRequestForTheWholeBatch()
        {
            // Arrange: no surrounding segments, so there is no window to slice for
            var texts = Enumerable.Range(0, 12).Select(i => $"t{i}").ToArray();

            // Act
            var batch = Assert.Single(DeepLTranslationProviderClient.BuildBatches(texts, null, null, " Maria is a woman. "));

            // Assert
            Assert.Equal(Enumerable.Range(0, 12), batch.TextIndices);
            Assert.Equal("Maria is a woman.", batch.Context);
        }

        [Fact]
        public void BuildBatches_CustomContextOnly_CustomContextCountsTowardTheRequestLimit()
        {
            // Arrange: two 60 KiB texts fit one request (120 KiB), but not with a 10 KiB custom context
            var texts = new[] { Text(60 * KiB), Text(60 * KiB) };
            var customContext = Text(10 * KiB);

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(texts, null, null, customContext).ToList();

            // Assert
            Assert.Equal(new[] { new[] { 0 }, new[] { 1 } }, batches.Select(b => b.TextIndices.ToArray()));
            Assert.All(batches, b => Assert.Equal(customContext, b.Context));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   \n ")]
        public void BuildBatches_CustomContextOnly_WhitespaceCustomContextSendsNoContext(string customContext)
        {
            // Act
            var batch = Assert.Single(DeepLTranslationProviderClient.BuildBatches(new[] { "a", "b" }, null, null, customContext));

            // Assert
            Assert.Null(batch.Context);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   \n ")]
        public void BuildBatches_WithContext_WhitespaceCustomContextIsIgnored(string customContext)
        {
            // Arrange
            var units = new[] { "a", "b" };

            // Act
            var batch = Assert.Single(DeepLTranslationProviderClient.BuildBatches(units, new[] { 0, 1 }, units, customContext));

            // Assert
            Assert.Equal("a\nb", batch.Context);
        }

        [Fact]
        public void BuildBatches_WithContext_IncludesContextOnlyNeighbours()
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
        public void BuildBatches_WithContext_CutsASliceEveryFiveUnits()
        {
            // Arrange
            var units = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l" };

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(units, Enumerable.Range(0, 12).ToArray(), units).ToList();

            // Assert
            Assert.Equal(new[] { new[] { 0, 1, 2, 3, 4 }, new[] { 5, 6, 7, 8, 9 }, new[] { 10, 11 } }, batches.Select(b => b.TextIndices.ToArray()));
        }

        [Fact]
        public void BuildBatches_WithContext_AddsThePreviousUnitButNothingAfter()
        {
            // Arrange
            var units = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i" };

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(units, Enumerable.Range(0, 9).ToArray(), units).ToList();

            // Assert
            Assert.Equal(new[] { "a\nb\nc\nd\ne", "e\nf\ng\nh\ni" }, batches.Select(b => b.Context));
        }

        [Fact]
        public void BuildBatches_WithContext_PaddingCountsTowardTheRequestLimit()
        {
            // Arrange: 30 KiB units; the previous unit in the context counts toward 128 KiB
            var units = Enumerable.Range(0, 4).Select(k => new string((char)('a' + k), 30 * KiB)).ToArray();

            // Act
            var batches = DeepLTranslationProviderClient.BuildBatches(units, new[] { 0, 1, 2, 3 }, units).ToList();

            // Assert
            Assert.Equal(new[] { new[] { 0, 1 }, new[] { 2 }, new[] { 3 } }, batches.Select(b => b.TextIndices.ToArray()));
            Assert.Equal(string.Join("\n", units[1], units[2]), batches[1].Context);
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
