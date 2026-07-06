using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LanguageWeaverProvider.Services;
using Sdl.LanguagePlatform.Core;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Offline unit tests for the pure Edge XLIFF batching seam (<c>EdgeXliffBatch</c>): build the
    /// single consolidated <c>application/x-xliff</c> request document from per-segment serializers,
    /// and split the consolidated response back into per-segment fragments by echoed trans-unit id.
    /// No files, no network — the production behavior was previously only validated by the live
    /// <c>TranslationRoundtripTest_Edge</c>.
    /// </summary>
    public class EdgeXliffBatchTests
    {
        private static readonly XNamespace Ns = "urn:oasis:names:tc:xliff:document:1.2";

        private static SegmentSerializer Serializer(Segment segment, string src = "eng", string tgt = "ger")
            => new(segment, src, tgt);

        private static IReadOnlyList<XElement> TransUnitsOf(string xliff)
            => XDocument.Parse(xliff).Descendants().Where(e => e.Name.LocalName == "trans-unit").ToList();

        // ---- Consolidate -------------------------------------------------------------------

        [Fact]
        public void Consolidate_EmptyList_ProducesParseableXliffWithNoTransUnits()
        {
            var result = EdgeXliffBatch.Consolidate(new List<SegmentSerializer>());

            Assert.Empty(TransUnitsOf(result));
        }

        [Fact]
        public void Consolidate_SingleSegment_ProducesOneTransUnitWithId1()
        {
            var serializer = Serializer(TestSegmentBuilder.New().Text("Hello").Build());

            var result = EdgeXliffBatch.Consolidate(new[] { serializer });

            var transUnits = TransUnitsOf(result);
            Assert.Single(transUnits);
            Assert.Equal("1", transUnits[0].Attribute("id")?.Value);
        }

        [Fact]
        public void Consolidate_MultipleSegments_AssignsSequentialIdsOneToN()
        {
            var serializers = new[]
            {
                Serializer(TestSegmentBuilder.New().Text("One").Build()),
                Serializer(TestSegmentBuilder.New().Text("Two").Build()),
                Serializer(TestSegmentBuilder.New().Text("Three").Build())
            };

            var result = EdgeXliffBatch.Consolidate(serializers);

            var ids = TransUnitsOf(result).Select(e => e.Attribute("id")?.Value).ToArray();
            Assert.Equal(new[] { "1", "2", "3" }, ids);
        }

        [Fact]
        public void Consolidate_SegmentWithTags_PreservesInlineGAndXMarkup()
        {
            var segment = TestSegmentBuilder.New()
                .Text("before ")
                .Paired(1, "bold")
                .Text(" ")
                .Placeholder(2)
                .Build();

            var result = EdgeXliffBatch.Consolidate(new[] { Serializer(segment) });

            var doc = XDocument.Parse(result);
            Assert.Single(doc.Descendants().Where(e => e.Name.LocalName == "g"));
            Assert.Single(doc.Descendants().Where(e => e.Name.LocalName == "x"));
        }

        [Fact]
        public void Consolidate_TakesSourceAndTargetLanguageFromFirstSerializer()
        {
            var serializers = new[]
            {
                Serializer(TestSegmentBuilder.New().Text("Hi").Build(), "eng", "ger"),
                Serializer(TestSegmentBuilder.New().Text("Yo").Build(), "eng", "fra")
            };

            var result = EdgeXliffBatch.Consolidate(serializers);

            var file = XDocument.Parse(result).Descendants().First(e => e.Name.LocalName == "file");
            Assert.Equal("eng", file.Attribute("source-language")?.Value);
            Assert.Equal("ger", file.Attribute("target-language")?.Value);
        }

        // ---- Split -------------------------------------------------------------------------

        [Fact]
        public void Split_IdsInOrder_AlignsFragmentsByIndex()
        {
            var response = Response((1, "Eins"), (2, "Zwei"));

            var fragments = EdgeXliffBatch.Split(response, 2);

            Assert.Equal(2, fragments.Length);
            Assert.Contains("Eins", fragments[0]);
            Assert.Contains("Zwei", fragments[1]);
        }

        [Fact]
        public void Split_MissingId_YieldsEmptyStringAtThatIndex()
        {
            var response = Response((1, "Eins")); // id 2 absent

            var fragments = EdgeXliffBatch.Split(response, 2);

            Assert.Equal(2, fragments.Length);
            Assert.Contains("Eins", fragments[0]);
            Assert.Equal(string.Empty, fragments[1]);
        }

        [Fact]
        public void Split_IdOutOfRange_IsIgnored()
        {
            var response = Response((1, "Eins"), (5, "Funf")); // 5 is out of range for count 2

            var fragments = EdgeXliffBatch.Split(response, 2);

            Assert.Equal(2, fragments.Length);
            Assert.Contains("Eins", fragments[0]);
            Assert.Equal(string.Empty, fragments[1]);
        }

        [Fact]
        public void Split_FragmentRetainsNestedAltTransTargetMarkup()
        {
            var response = Response((1, "Hallo"));

            var fragments = EdgeXliffBatch.Split(response, 1);

            Assert.Contains("target", fragments[0]);
            Assert.Contains("Hallo", fragments[0]);
        }

        // Real consolidated XLIFF download captured live from the Edge API (QE-capable pair
        // EngFra_Generative-Gen-32888_Cloud). Edge embeds per-segment QE exactly like Cloud: as the
        // match-quality attribute of <alt-trans>. This pins the contract EdgeService relies on when it
        // extracts QE from each split fragment via SegmentSerializer.ExtractQualityEstimation.
        private const string EdgeQeDownload =
            "<?xml version=\"1.0\" ?><xliff version=\"1.2\" xmlns=\"urn:oasis:names:tc:xliff:document:1.2\"><file original=\"segment\" source-language=\"en\" target-language=\"fr\" datatype=\"plaintext\"><header><tool tool-name=\"SDL ETS\" tool-version=\"8.7.2\" tool-id=\"ETS\"></tool></header><body><trans-unit id=\"1\"><source>This is a simple sentence.</source><alt-trans tool-id=\"ETS\" date=\"2026-06-25T15:59:52Z\" match-quality=\"Good\"><target xml:lang=\"fr\">C'est une phrase simple.</target></alt-trans></trans-unit><trans-unit id=\"2\"><source>The quick brown fox jumps over the lazy dog every single morning.</source><alt-trans tool-id=\"ETS\" date=\"2026-06-25T15:59:52Z\" match-quality=\"Adequate\"><target xml:lang=\"fr\">Le renard brun rapide saute par-dessus le chien paresseux chaque matin.</target></alt-trans></trans-unit></body></file></xliff>";

        [Fact]
        public void Split_PreservesPerSegmentQualityEstimation_FromLiveEdgeDownload()
        {
            var fragments = EdgeXliffBatch.Split(EdgeQeDownload, 2);

            Assert.Equal("Good", SegmentSerializer.ExtractQualityEstimation(fragments[0]));
            Assert.Equal("Adequate", SegmentSerializer.ExtractQualityEstimation(fragments[1]));
        }

        private static string Response(params (int id, string targetText)[] units)
        {
            var body = new XElement(Ns + "body");
            foreach (var (id, targetText) in units)
            {
                body.Add(new XElement(Ns + "trans-unit",
                    new XAttribute("id", id.ToString()),
                    new XElement(Ns + "source", "source"),
                    new XElement(Ns + "alt-trans",
                        new XElement(Ns + "target", targetText))));
            }

            var xliff = new XElement(Ns + "xliff",
                new XAttribute("version", "1.2"),
                new XElement(Ns + "file",
                    new XAttribute("source-language", "eng"),
                    new XAttribute("target-language", "ger"),
                    body));

            return xliff.ToString(SaveOptions.DisableFormatting);
        }
    }
}
