using Sdl.Community.AntidoteVerifier.Extensions;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Xunit;

namespace Sdl.Community.AntidoteVerifier.Tests
{
    /// <summary>
    /// A placeholder tag must contribute NO characters to the text sent to Antidote. Studio's
    /// DisplayText is a label for the tag ("fn 1" for a DOCX footnote reference), not document
    /// content: collecting it put a space into the corrector's copy that the document does not
    /// contain, and welded the label onto the neighbouring word -- SRQ-32047 issue 3.
    /// Because the tag occupies no characters, the text on either side of it is adjacent and the
    /// tag survives only as a zero-width boundary, so both halves are pinned here.
    /// Built from the real Studio markup types rather than fakes -- the interfaces are wide, and the
    /// framework already ships constructible implementations.
    /// </summary>
    public class PlaceholderTagTextTests
    {
        // Studio's display text for a DOCX footnote reference. The space inside it is the one the
        // translator saw flagged as superfluous.
        private const string FootnoteDisplayText = "fn 1";

        private const string Before = "La proposition du Conseil";
        private const string After = ", ainsi que";
        private const int Boundary = 25; // == Before.Length, where the tag sits

        [Fact]
        public void Placeholder_tag_contributes_no_characters()
        {
            Assert.Equal(Before.Length, Boundary);
            Assert.Equal(Before + After, Segment().GetString());
        }

        // The guard replacing the protection the old locked range gave: ReplaceText rewrites the
        // IText elements either side and leaves the tag in place, so a correction CROSSING the tag
        // would silently re-anchor a footnote to a different word.
        [Theory]
        [InlineData(20, 30, true)]   // crosses the tag -> refused
        [InlineData(0, 36, true)]    // whole segment, crosses it -> refused
        [InlineData(18, 25, false)]  // ends exactly AT the tag -> allowed
        [InlineData(25, 30, false)]  // starts exactly AT the tag -> allowed
        [InlineData(0, 10, false)]   // nowhere near it -> allowed
        public void Only_a_correction_that_crosses_the_tag_is_refused(int start, int end, bool refused)
        {
            var segment = Segment();
            var visitor = new CustomTextCollectionVisitor(segment, start, end);
            foreach (var item in segment)
                item.AcceptVisitor(visitor);

            Assert.Equal(refused, visitor.RangeContainsTextLocked());
        }

        // "Conseil" + [fn 1] + ", ainsi que", the shape that produced the phantom space.
        private static ISegment Segment()
        {
            var items = new DocumentItemFactory();
            var properties = new PropertiesFactory();

            var tagProperties = properties.CreatePlaceholderTagProperties(FootnoteDisplayText);
            tagProperties.DisplayText = FootnoteDisplayText;

            var segment = items.CreateSegment(items.CreateSegmentPairProperties());
            segment.Add(items.CreateText(properties.CreateTextProperties(Before)));
            segment.Add(items.CreatePlaceholderTag(tagProperties));
            segment.Add(items.CreateText(properties.CreateTextProperties(After)));
            return segment;
        }
    }
}
