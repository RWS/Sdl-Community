using System.Linq;
using LanguageWeaverProvider.Services;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Xunit;

namespace LanguageWeaverProviderTests
{
    public class SegmentTagPlacerTests
    {
        private static readonly CultureCode TargetCulture = new CultureCode("de-DE");

        [Fact]
        public void PreparedText_PlainSegment_ReturnsTextUnchanged()
        {
            var segment = new Segment();
            segment.Add("Hello world");

            var placer = new SegmentTagPlacer(segment);

            Assert.Equal("Hello world", placer.PreparedText);
        }

        [Fact]
        public void PreparedText_SegmentWithSingleTag_EmitsIndexedPlaceholder()
        {
            var segment = new Segment();
            segment.Add("Click ");
            segment.Add(new Tag(TagType.Start, "1", 1));
            segment.Add("here");
            segment.Add(new Tag(TagType.End, "1", 1));
            segment.Add(".");

            var placer = new SegmentTagPlacer(segment);

            Assert.Equal("Click <x id=\"0\"/>here<x id=\"1\"/>.", placer.PreparedText);
        }

        [Fact]
        public void PreparedText_MultipleTags_AssignsSequentialIndices()
        {
            var segment = new Segment();
            segment.Add(new Tag(TagType.Start, "1", 1));
            segment.Add("a");
            segment.Add(new Tag(TagType.Start, "2", 2));
            segment.Add("b");
            segment.Add(new Tag(TagType.End, "2", 2));
            segment.Add(new Tag(TagType.End, "1", 1));

            var placer = new SegmentTagPlacer(segment);

            Assert.Equal("<x id=\"0\"/>a<x id=\"1\"/>b<x id=\"2\"/>", placer.PreparedText);
        }

        [Fact]
        public void BuildTargetSegment_PlainTranslation_ProducesSingleTextElement()
        {
            var source = new Segment();
            source.Add("Hello");
            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment("Hallo", TargetCulture);

            Assert.Equal(TargetCulture, target.Culture);
            Assert.False(target.HasTags);
            Assert.Equal("Hallo", target.ToPlain());
        }

        [Fact]
        public void BuildTargetSegment_PreservesOriginalTagInstance()
        {
            var startTag = new Tag(TagType.Start, "1", 1);
            var source = new Segment();
            source.Add(startTag);
            source.Add("text");

            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment("<x id=\"0\"/>texte", TargetCulture);

            var emittedTag = target.Elements.OfType<Tag>().Single();
            Assert.Same(startTag, emittedTag);
        }

        [Fact]
        public void BuildTargetSegment_TagAtEndOfTranslation_RestoresTag()
        {
            var endTag = new Tag(TagType.End, "1", 1);
            var source = new Segment();
            source.Add("hello");
            source.Add(endTag);

            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment("hallo<x id=\"0\"/>", TargetCulture);

            var elements = target.Elements;
            Assert.Equal(2, elements.Count);
            Assert.IsType<Text>(elements[0]);
            Assert.Same(endTag, elements[1]);
        }

        [Fact]
        public void BuildTargetSegment_TagsReorderedByMt_PlacesTagsAtNewPositions()
        {
            var open = new Tag(TagType.Start, "1", 1);
            var close = new Tag(TagType.End, "1", 1);
            var source = new Segment();
            source.Add(open);
            source.Add("bold");
            source.Add(close);
            source.Add(" plain");

            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment("plain <x id=\"0\"/>fett<x id=\"1\"/>", TargetCulture);

            var elements = target.Elements;
            Assert.Equal(4, elements.Count);
            Assert.Equal("plain ", ((Text)elements[0]).Value);
            Assert.Same(open, elements[1]);
            Assert.Equal("fett", ((Text)elements[2]).Value);
            Assert.Same(close, elements[3]);
        }

        [Fact]
        public void BuildTargetSegment_DroppedPlaceholder_OmitsTagWithoutThrowing()
        {
            var tag = new Tag(TagType.Start, "1", 1);
            var source = new Segment();
            source.Add(tag);
            source.Add("text");

            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment("texte", TargetCulture);

            Assert.False(target.HasTags);
            Assert.Equal("texte", target.ToPlain());
        }

        [Fact]
        public void BuildTargetSegment_OutOfRangePlaceholder_IsIgnored()
        {
            var source = new Segment();
            source.Add("hello");
            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment("hello <x id=\"42\"/>world", TargetCulture);

            Assert.False(target.HasTags);
            Assert.Equal("hello world", target.ToPlain());
        }

        [Fact]
        public void BuildTargetSegment_EmptyTranslation_ReturnsEmptyTargetSegment()
        {
            var source = new Segment();
            source.Add("source");
            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment(string.Empty, TargetCulture);

            Assert.Equal(TargetCulture, target.Culture);
            Assert.Empty(target.Elements);
        }

        [Fact]
        public void BuildTargetSegment_NullTranslation_ReturnsEmptyTargetSegment()
        {
            var source = new Segment();
            source.Add("source");
            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment(null, TargetCulture);

            Assert.Empty(target.Elements);
        }

        [Fact]
        public void RoundTrip_IdenticalTranslation_PreservesTagsAndText()
        {
            var open = new Tag(TagType.Start, "1", 1);
            var close = new Tag(TagType.End, "1", 1);
            var source = new Segment();
            source.Add("Click ");
            source.Add(open);
            source.Add("here");
            source.Add(close);
            source.Add(".");

            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment(placer.PreparedText, TargetCulture);

            var elements = target.Elements;
            Assert.Equal(5, elements.Count);
            Assert.Equal("Click ", ((Text)elements[0]).Value);
            Assert.Same(open, elements[1]);
            Assert.Equal("here", ((Text)elements[2]).Value);
            Assert.Same(close, elements[3]);
            Assert.Equal(".", ((Text)elements[4]).Value);
        }

        [Fact]
        public void PreparedText_TextWithXmlSpecialChars_IsEscaped()
        {
            var segment = new Segment();
            segment.Add("a < b & c > d \"quote\"");

            var placer = new SegmentTagPlacer(segment);

            Assert.Equal("a &lt; b &amp; c &gt; d &quot;quote&quot;", placer.PreparedText);
        }

        [Fact]
        public void BuildTargetSegment_EncodedTextSpan_IsDecoded()
        {
            var source = new Segment();
            source.Add("dummy");
            var placer = new SegmentTagPlacer(source);

            var target = placer.BuildTargetSegment("a &lt; b &amp; c &gt; d &quot;quote&quot;", TargetCulture);

            Assert.Equal("a < b & c > d \"quote\"", target.ToPlain());
        }

        [Fact]
        public void RoundTrip_TextWithXmlSpecialChars_PreservesOriginalText()
        {
            var tag = new Tag(TagType.Start, "1", 1);
            var source = new Segment();
            source.Add("a < b & c");
            source.Add(tag);
            source.Add(" \"d\" > e");

            var placer = new SegmentTagPlacer(source);
            var target = placer.BuildTargetSegment(placer.PreparedText, TargetCulture);

            var elements = target.Elements;
            Assert.Equal(3, elements.Count);
            Assert.Equal("a < b & c", ((Text)elements[0]).Value);
            Assert.Same(tag, elements[1]);
            Assert.Equal(" \"d\" > e", ((Text)elements[2]).Value);
        }
    }
}
