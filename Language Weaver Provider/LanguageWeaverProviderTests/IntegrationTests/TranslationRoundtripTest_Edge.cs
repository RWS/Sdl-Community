using LanguageWeaverProvider.Services;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LanguageWeaverProviderTests.IntegrationTests
{
    //IntegrationTests: translation WF against the Language Weaver EDGE engine.
    // Mirrors TranslationRoundtripTest (Cloud) but targets the Edge async API:
    //   POST /api/v2/translations  ->  poll GET /api/v2/translations/{id}  ->  GET .../download (base64)
    //
    // Edge preserves inline <g>/<x> tags only under inputFormat = "application/x-xliff" (verified by
    // EdgeInputFormatExperiment/EdgeBatchResponseShapeExperiment). So this test uses the SAME
    // SegmentSerializer XLIFF as Cloud, submits it under that MIME, deserializes the <target> back,
    // and asserts the tag anchor/type multisets survive — exactly mirroring the Cloud test.
    public class TranslationRoundtripTest_Edge
    {
        private const string EdgeHost = "https://mt01.edge.languageweaver.com";

        private const string AccessToken =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3ODI1MTQwOTYsInRva2VuIjoiNjQ0ZmMwZTEtOTgyZi00MjAwLWIwNDgtMzBjNGE5MzQwMDZmIn0.G0Ccm9V5ytvEzYYevLYysjn4-d7oIkX_h0aw8W93kmmvvAJDfNxZTP_9ElPrxR1kUR8m4pMFEf1TXjh6UhWnawr2cyMpQyB7bAKFs6ZHlCzHe5AiajNIaXkQVLOD_2ZwLXVqcxhdkKwfsnUmqiJJOmNYW62ZVizliYlb0wJ_lWltjZQ1G54iXtXHEkYRVc8pDBEPC91Cg0In9aYex4cs6KpMIkwWzlHvh3LgDF4iYQqZIzav_aM2dLW6e1IZQuFfyb_8NVLblHrQFiYarCYU2OVhbQx-YuEwPQa1pJ2Ck3QYfLSPle9P-v5GHK1AuoNFbVm9pf1FPR6Uhe8QQs0CLw";

        private const string LanguagePairId = "EngGer_AutoAdaptive_SRV_TNM";
        private const string SourceLang = "eng";
        private const string TargetLang = "ger";

        [Theory]
        [ClassData(typeof(InputData))]
        public void TranslateRountripTest(Segment sourceSegment)
        {
            // Edge round-trips inline tags only when fed the SegmentSerializer XLIFF under
            // application/x-xliff — identical to the Cloud production serialization.
            var segmentSerializer = new SegmentSerializer(sourceSegment, SourceLang, TargetLang);
            var segmentString = segmentSerializer.SerializedSegment;

            var targetString = Translate(segmentString);
            var targetSegment = segmentSerializer.DeserializeSegment(targetString);

            // validate target segment
            Assert.NotNull(targetSegment);
            Assert.False(string.IsNullOrEmpty(targetSegment.ToString()));

            var sourceTags = sourceSegment.Elements.OfType<Tag>().ToList();
            var targetTags = targetSegment.Elements.OfType<Tag>().ToList();

            // Tag order can differ across languages; compare anchors and types as multisets.
            Assert.Equal(sourceTags.Count, targetTags.Count);
            Assert.Equal(
                sourceTags.GroupBy(t => t.Anchor).ToDictionary(g => g.Key, g => g.Count()),
                targetTags.GroupBy(t => t.Anchor).ToDictionary(g => g.Key, g => g.Count()));
            Assert.Equal(
                sourceTags.GroupBy(t => t.Type).ToDictionary(g => g.Key, g => g.Count()),
                targetTags.GroupBy(t => t.Type).ToDictionary(g => g.Key, g => g.Count()));
        }

        private string Translate(string segmentString)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");

            // Step 1 — Submit. Edge expects an application/x-www-form-urlencoded body (see
            // EdgeService.SendTranslationRequest), NOT JSON, with a single base64-encoded input.
            // inputFormat = "application/x-xliff" is the only format that preserves inline tags.
            var fields = new Dictionary<string, string>
            {
                ["languagePairId"] = LanguagePairId,
                ["title"]          = "Trados Studio",
                ["input"]          = Base64Encode(segmentString),
                ["inputFormat"]    = "application/x-xliff"
            };

            var submitResponse = client
                .PostAsync(
                    $"{EdgeHost}/api/v2/translations",
                    new FormUrlEncodedContent(fields))
                .GetAwaiter().GetResult();

            submitResponse.EnsureSuccessStatusCode();
            var submitJson    = submitResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var translationId = JObject.Parse(submitJson)["translationId"].ToString();

            // Step 2 — Poll until state == done (matches EdgeService.WaitForTranslationCompletion).
            string state;
            do
            {
                Thread.Sleep(1000);
                var statusResponse = client
                    .GetAsync($"{EdgeHost}/api/v2/translations/{translationId}")
                    .GetAwaiter().GetResult();
                statusResponse.EnsureSuccessStatusCode();
                var statusJson = statusResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var status     = JObject.Parse(statusJson);

                state = status["state"]?.ToString();

                var error = status["error"];
                if (error is not null && error.Type != JTokenType.Null)
                    throw new Exception($"Edge translation failed: {error}");
            }
            while (!state.Equals("done", StringComparison.OrdinalIgnoreCase));

            // Step 3 — Download the translated output (raw base64 body, not JSON).
            var downloadResponse = client
                .GetAsync($"{EdgeHost}/api/v2/translations/{translationId}/download")
                .GetAwaiter().GetResult();
            downloadResponse.EnsureSuccessStatusCode();
            var encoded = downloadResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return Base64Decode(encoded);
        }

        private static string Base64Encode(string text)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

        private static string Base64Decode(string encoded)
            => Encoding.UTF8.GetString(Convert.FromBase64String(encoded.Trim()));
    }
}
