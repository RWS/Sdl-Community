using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LanguageWeaverProvider.Services;
using Sdl.LanguagePlatform.Core;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Offline characterization tests for <see cref="SegmentSerializer"/> — the serialize → engine →
    /// deserialize round-trip is exercised entirely in memory by echoing the serialized
    /// <c>&lt;source&gt;</c> back as a <c>&lt;target&gt;</c> (a perfect translation). This pins the
    /// behavior the live <c>TranslationRoundtripTest</c>/<c>_Edge</c> only assert against a real API:
    /// inline tags survive as <c>&lt;g&gt;</c>/<c>&lt;x&gt;</c> and the original tag anchors/types are
    /// recovered by anchor lookup.
    /// </summary>
    public class SegmentSerializerTests
    {
        private static readonly XNamespace Ns = "urn:oasis:names:tc:xliff:document:1.2";

        private static SegmentSerializer Serializer(Segment segment, string src = "eng", string tgt = "ger")
            => new(segment, src, tgt);

        // ---- Serialization shape -----------------------------------------------------------

        [Fact]
        public void SerializedSegment_CarriesSourceAndTargetLanguage()
        {
            var serializer = Serializer(TestSegmentBuilder.New().Text("Hello").Build(), "eng", "fra");

            var file = XDocument.Parse(serializer.SerializedSegment)
                .Descendants().First(e => e.Name.LocalName == "file");

            Assert.Equal("eng", file.Attribute("source-language")?.Value);
            Assert.Equal("fra", file.Attribute("target-language")?.Value);
        }

        [Fact]
        public void SerializedSegment_PairedTag_BecomesNestedGElement()
        {
            var serializer = Serializer(TestSegmentBuilder.New().Paired(7, "bold").Build());

            var doc = XDocument.Parse(serializer.SerializedSegment);
            var g = doc.Descendants().Single(e => e.Name.LocalName == "g");

            Assert.Equal("7", g.Attribute("id")?.Value);
            Assert.Equal("bold", g.Value);
        }

        [Fact]
        public void SerializedSegment_StandaloneTag_BecomesXElement()
        {
            var serializer = Serializer(TestSegmentBuilder.New().Placeholder(3).Build());

            var doc = XDocument.Parse(serializer.SerializedSegment);
            var x = doc.Descendants().Single(e => e.Name.LocalName == "x");

            Assert.Equal("3", x.Attribute("id")?.Value);
        }

        // ---- Deserialization edge cases ----------------------------------------------------

        [Fact]
        public void DeserializeSegment_EmptyResponse_ReturnsEmptySegment()
        {
            var serializer = Serializer(TestSegmentBuilder.New().Text("Hello").Build());

            var result = serializer.DeserializeSegment(string.Empty);

            Assert.NotNull(result);
            Assert.Empty(result.Elements);
        }

        [Fact]
        public void DeserializeSegment_ResponseWithoutTarget_Throws()
        {
            var serializer = Serializer(TestSegmentBuilder.New().Text("Hello").Build());

            // The serialized request has a <source> but no <target>, so it is a valid "no target" doc.
            Assert.Throws<System.InvalidOperationException>(
                () => serializer.DeserializeSegment(serializer.SerializedSegment));
        }

        // ---- Full in-memory round-trip (Anchor-reuse assumption) ---------------------------

        [Fact]
        public void RoundTrip_PlainText_PreservesText()
        {
            var serializer = Serializer(TestSegmentBuilder.New().Text("Hello world").Build());

            var result = serializer.DeserializeSegment(EchoAsTarget(serializer));

            Assert.Equal("Hello world", result.ToString());
        }

        [Fact]
        public void RoundTrip_PairedTag_PreservesAnchorAndType()
        {
            var serializer = Serializer(TestSegmentBuilder.New()
                .Text("a ").Paired(5, "b").Text(" c").Build());

            var result = serializer.DeserializeSegment(EchoAsTarget(serializer));

            var tags = result.Elements.OfType<Tag>().ToList();
            Assert.Equal(new[] { TagType.Start, TagType.End }, tags.Select(t => t.Type));
            Assert.True(tags.All(t => t.Anchor == 5));
        }

        [Fact]
        public void RoundTrip_StandaloneTag_PreservesAnchorAndType()
        {
            var serializer = Serializer(TestSegmentBuilder.New()
                .Text("a ").Placeholder(9).Text(" b").Build());

            var result = serializer.DeserializeSegment(EchoAsTarget(serializer));

            var tag = Assert.Single(result.Elements.OfType<Tag>());
            Assert.Equal(TagType.Standalone, tag.Type);
            Assert.Equal(9, tag.Anchor);
        }

        [Fact]
        public void RoundTrip_MixedTags_PreservesTagAnchorAndTypeMultisets()
        {
            var source = TestSegmentBuilder.New()
                .Text("before ")
                .Paired(1, "bold")
                .Text(" mid ")
                .Placeholder(2)
                .Text(" ")
                .Paired(3, "italic")
                .Build();
            var serializer = Serializer(source);

            var result = serializer.DeserializeSegment(EchoAsTarget(serializer));

            var sourceTags = source.Elements.OfType<Tag>().ToList();
            var resultTags = result.Elements.OfType<Tag>().ToList();

            Assert.Equal(sourceTags.Count, resultTags.Count);
            Assert.Equal(
                sourceTags.GroupBy(t => t.Anchor).ToDictionary(g => g.Key, g => g.Count()),
                resultTags.GroupBy(t => t.Anchor).ToDictionary(g => g.Key, g => g.Count()));
            Assert.Equal(
                sourceTags.GroupBy(t => t.Type).ToDictionary(g => g.Key, g => g.Count()),
                resultTags.GroupBy(t => t.Type).ToDictionary(g => g.Key, g => g.Count()));
        }

        [Fact]
        public void RoundTrip_ReusesOriginalTagInstancesByAnchor()
        {
            var startTag = new Tag(TagType.Start, "4", 4);
            var endTag = new Tag(TagType.End, "4", 4);
            var segment = new Segment();
            segment.Add(startTag);
            segment.Add("x");
            segment.Add(endTag);
            var serializer = Serializer(segment);

            var result = serializer.DeserializeSegment(EchoAsTarget(serializer));

            var tags = result.Elements.OfType<Tag>().ToList();
            Assert.Same(startTag, tags[0]);
            Assert.Same(endTag, tags[1]);
        }

        // ---- Quality estimation extraction -------------------------------------------------

        // Real XLIFF captured from the live Cloud API (model=genericqe) — QE travels with the
        // translated content as the match-quality attribute of <alt-trans>.
        private const string GoodQeXliff =
            "<?xml version=\"1.0\" ?><xliff version=\"1.2\" xmlns=\"urn:oasis:names:tc:xliff:document:1.2\"><file original=\"segment\" source-language=\"en\" target-language=\"de\" datatype=\"plaintext\"><header><tool tool-name=\"SDL ETS\" tool-version=\"8.7.1\" tool-id=\"ETS\"></tool></header><body><trans-unit id=\"1\"><source>The quick brown fox jumps over the lazy dog.</source><alt-trans tool-id=\"ETS\" date=\"2026-06-25T15:25:56Z\" match-quality=\"Good\"><target xml:lang=\"de\">Der schnelle braune Fuchs springt &#xFC;ber den faulen Hund.</target></alt-trans></trans-unit></body></file></xliff>";

        private const string PoorQeXliff =
            "<?xml version=\"1.0\" ?><xliff version=\"1.2\" xmlns=\"urn:oasis:names:tc:xliff:document:1.2\"><file original=\"segment\" source-language=\"en\" target-language=\"de\" datatype=\"plaintext\"><header><tool tool-name=\"SDL ETS\" tool-version=\"8.7.1\" tool-id=\"ETS\"></tool></header><body><trans-unit id=\"1\"><source>asdf qwer zxcv hjkl nonsense gibberish.</source><alt-trans tool-id=\"ETS\" date=\"2026-06-25T15:25:56Z\" match-quality=\"Poor\"><target xml:lang=\"de\">asdf qwer zxcv hjkl Unsense Gibberish.</target></alt-trans></trans-unit></body></file></xliff>";

        // Real XLIFF captured from the live Cloud API (model=generic) — no QE requested/emitted,
        // so <alt-trans> carries no match-quality attribute.
        private const string NoQeXliff =
            "<?xml version=\"1.0\" ?><xliff version=\"1.2\" xmlns=\"urn:oasis:names:tc:xliff:document:1.2\"><file original=\"segment\" source-language=\"en\" target-language=\"de\" datatype=\"plaintext\"><header><tool tool-name=\"SDL ETS\" tool-version=\"8.7.1\" tool-id=\"ETS\"></tool></header><body><trans-unit id=\"1\"><source>The quick brown fox jumps over the lazy dog.</source><alt-trans tool-id=\"ETS\" date=\"2026-06-25T15:25:54Z\"><target xml:lang=\"de\">Der schnelle braune Fuchs springt &#xFC;ber den faulen Hund.</target></alt-trans></trans-unit></body></file></xliff>";

        [Theory]
        [InlineData(GoodQeXliff, "Good")]
        [InlineData(PoorQeXliff, "Poor")]
        public void ExtractQualityEstimation_ReadsMatchQualityFromAltTrans(string xliff, string expected)
        {
            Assert.Equal(expected, SegmentSerializer.ExtractQualityEstimation(xliff));
        }

        [Fact]
        public void ExtractQualityEstimation_ReturnsNull_WhenNoMatchQualityAttribute()
        {
            Assert.Null(SegmentSerializer.ExtractQualityEstimation(NoQeXliff));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ExtractQualityEstimation_ReturnsNull_ForEmptyInput(string xliff)
        {
            Assert.Null(SegmentSerializer.ExtractQualityEstimation(xliff));
        }

        /// <summary>
        /// Simulates a perfect engine translation: deep-clones the serialized <c>&lt;source&gt;</c>
        /// content into a <c>&lt;target&gt;</c> (nested in <c>&lt;alt-trans&gt;</c> to mirror the Edge
        /// response shape) and returns a parseable XLIFF document.
        /// </summary>
        private static string EchoAsTarget(SegmentSerializer serializer)
        {
            var doc = XDocument.Parse(serializer.SerializedSegment);
            var source = doc.Descendants().First(e => e.Name.LocalName == "source");

            var target = new XElement(Ns + "target");
            foreach (var node in source.Nodes())
                target.Add(node); // parented nodes are cloned on Add, leaving the source intact

            var xliff = new XElement(Ns + "xliff",
                new XAttribute("version", "1.2"),
                new XElement(Ns + "file",
                    new XAttribute("source-language", serializer.SourceLanguage),
                    new XAttribute("target-language", serializer.TargetLanguage),
                    new XElement(Ns + "body",
                        new XElement(Ns + "trans-unit",
                            new XAttribute("id", "1"),
                            new XElement(Ns + "alt-trans", target)))));

            return xliff.ToString(SaveOptions.DisableFormatting);
        }
    }
}
