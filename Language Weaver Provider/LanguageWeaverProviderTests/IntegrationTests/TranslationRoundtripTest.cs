using LanguageWeaverProvider.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.Core;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Xunit;

namespace LanguageWeaverProviderTests.IntegrationTests
{
    //IntegrationTests: translation WF
    public class TranslationRoundtripTest
    {
        private const string AccessToken =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IlJUbEZSVGMxTnpCRE9EbEdSRFl5TVRsRk1rUTNPRGhEUWtSRFFUTXhSRVUxTlVVME1EUXhRUSJ9.eyJodHRwczovL3NkbC5jb20vZW1haWwiOiJlYWxidUByd3MuY29tIiwiaHR0cHM6Ly9yd3MuY29tL2Nvbm5lY3Rpb24iOiJSV1MtQUFEIiwiaXNzIjoiaHR0cHM6Ly9zZGwtcHJvZC5ldS5hdXRoMC5jb20vIiwic3ViIjoic2FtbHB8UldTLUFBRHxlYWxidUBzZGwuY29tIiwiYXVkIjpbImh0dHBzOi8vYXBpLnNkbC5jb20iLCJodHRwczovL3NkbC1wcm9kLmV1LmF1dGgwLmNvbS91c2VyaW5mbyJdLCJpYXQiOjE3ODIzMDgyOTcsImV4cCI6MTc4MjM5NDY5Nywic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCBvZmZsaW5lX2FjY2VzcyIsImF6cCI6IkY0TnBPR0cxc0JhRXprMzc5TTZaeFgzZ0dhMGlIMUZmIn0.L2AbzFcahfmCpp35xatkHNm-EIZNVDKRBbaqT9Rq2XGPxKNs3Zr42zuZvw0Bm0HkSlZlao6p2ycZgV_gtF5Biz7mIZz7ecp9Kxf89R8D3bU-ueqb8Gno2DX-p2fghOEsYi5qZ1E07vaTWa7d8nPTn79kKEXAifWbSY5GVGosLGXHIuk6gwpLBcR5KvSblFMvuSSU_WGRJdNLb1I-dft0qDRzcBZ2LmHYj6lioLO31POtTk60yPKxYQDu1YquWtO74KACYqmuv9LWDYyfkmhlncwKRUg5pOjnjuJGECTkM7NQeH2uLM6H3rrC-l6QwqQkwrDLRXyLd_zWY_GNpVY8Fg";

        private const string SourceLang = "eng";
        private const string TargetLang  = "ger";
        private const string Model       = "generic";

        [Theory]
        [ClassData(typeof(InputData))]
        public void TranslateRountripTest(Segment sourceSegment)
        {
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

            // Step 1 — Submit
            var submitBody = JsonConvert.SerializeObject(new
            {
                sourceLanguageId = SourceLang,
                targetLanguageId = TargetLang,
                model            = Model,
                input            = new[] { segmentString },
                inputFormat      = "XLIFF"
            });

            var submitResponse = client
                .PostAsync(
                    "https://api.languageweaver.com/v4/mt/translations/async",
                    new StringContent(submitBody, Encoding.UTF8, "application/json"))
                .GetAwaiter().GetResult();

            submitResponse.EnsureSuccessStatusCode();
            var submitJson = submitResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var requestId  = JObject.Parse(submitJson)["requestId"].ToString();

            // Step 2 — Poll until DONE or FAILED
            string status;
            do
            {
                Thread.Sleep(1000);
                var statusResponse = client
                    .GetAsync($"https://api.languageweaver.com/v4/mt/translations/async/{requestId}")
                    .GetAwaiter().GetResult();
                statusResponse.EnsureSuccessStatusCode();
                var statusJson = statusResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                status = JObject.Parse(statusJson)["translationStatus"].ToString();
            }
            while (status.Equals("INIT",        StringComparison.OrdinalIgnoreCase)
                || status.Equals("TRANSLATING", StringComparison.OrdinalIgnoreCase));

            if (!status.Equals("DONE", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Translation failed with status: {status}");

            // Step 3 — Retrieve content
            var contentResponse = client
                .GetAsync($"https://api.languageweaver.com/v4/mt/translations/async/{requestId}/content")
                .GetAwaiter().GetResult();
            contentResponse.EnsureSuccessStatusCode();
            var contentJson = contentResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JObject.Parse(contentJson)["translation"][0].ToString();
        }
    }
}