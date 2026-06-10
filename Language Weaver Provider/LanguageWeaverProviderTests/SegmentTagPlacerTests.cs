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

            Assert.Equal("Click <lwtg0/>here<lwtg1/>.", placer.PreparedText);
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

            Assert.Equal("<lwtg0/>a<lwtg1/>b<lwtg2/><lwtg3/>", placer.PreparedText);
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

            var target = placer.BuildTargetSegment("<lwtg0/>texte", TargetCulture);

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

            var target = placer.BuildTargetSegment("hallo<lwtg0/>", TargetCulture);

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

            var target = placer.BuildTargetSegment("plain <lwtg0/>fett<lwtg1/>", TargetCulture);

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

            var target = placer.BuildTargetSegment("hello <lwtg42/>world", TargetCulture);

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
    }
}
