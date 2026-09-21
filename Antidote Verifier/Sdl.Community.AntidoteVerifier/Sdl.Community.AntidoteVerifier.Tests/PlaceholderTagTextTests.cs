using Sdl.Community.AntidoteVerifier.Extensions;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
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

        // A zero-length insertion AT the tag boundary is the shape of every French "space before
        // punctuation" correction next to a footnote reference ("abordable[fn]; ils"). Both text
        // elements meet the insertion point, and applying it to the wrong one wiped the whole first
        // element: "Les États membres devraient soutenir le logement abordable" vanished from a
        // Studio segment live (2026-09-21). The space belongs in front of the character Antidote
        // pointed at -- the punctuation after the tag -- and nothing else may change.
        [Fact]
        public void Insertion_at_the_tag_boundary_keeps_both_halves_and_lands_after_the_tag()
        {
            var segment = Segment();

            segment.Replace(Boundary, Boundary, " ");

            Assert.Equal(Before + " " + After, segment.GetString());
            Assert.Equal(Before, ((IText)segment[0]).Properties.Text);
            Assert.Equal(" " + After, ((IText)segment[2]).Properties.Text);
        }

        // Every other shape of edit the corrector sends must be untouched by the boundary rule.
        [Theory]
        [InlineData(0, 0, "x", "xLa proposition du Conseil", ", ainsi que")]        // insert at segment start
        [InlineData(3, 3, "x", "La xproposition du Conseil", ", ainsi que")]       // insert inside first element
        [InlineData(30, 30, "x", "La proposition du Conseil", ", ainxsi que")]     // insert inside second element
        [InlineData(18, 25, "Parlement", "La proposition du Parlement", ", ainsi que")] // replace ending AT the tag
        [InlineData(25, 26, ";", "La proposition du Conseil", "; ainsi que")]      // replace starting AT the tag
        [InlineData(0, 25, "Bref", "Bref", ", ainsi que")]                         // replace a whole element
        [InlineData(26, 32, "", "La proposition du Conseil", ", que")]             // delete inside second element
        public void Other_edits_land_in_the_element_that_owns_the_range(int start, int end, string replacement,
            string first, string last)
        {
            var segment = Segment();

            segment.Replace(start, end, replacement);

            Assert.Equal(first, ((IText)segment[0]).Properties.Text);
            Assert.Equal(last, ((IText)segment[2]).Properties.Text);
            Assert.Equal(first + last, segment.GetString());
        }

        [Fact]
        public void Insertion_at_the_end_of_the_segment_appends_to_the_last_element()
        {
            var segment = Segment();

            segment.Replace(36, 36, "!");

            Assert.Equal(Before + After + "!", segment.GetString());
        }

        // The boundary rule hands a zero-length insertion to the LATER element. When that element is
        // locked content the lock check must refuse it, exactly as it refuses a non-empty edit inside
        // the lock -- otherwise the space is written into text the translator cannot touch.
        [Theory]
        [InlineData(25, true)]   // in front of the locked run -> lands inside it -> refused
        [InlineData(36, true)]   // at the very end, locked run is last -> appends to it -> refused
        [InlineData(10, false)]  // inside the unlocked run -> allowed
        public void Insertion_that_would_land_in_locked_content_is_refused(int position, bool refused)
        {
            var segment = SegmentWithLockedTail();
            var visitor = new CustomTextCollectionVisitor(segment, position, position);
            foreach (var item in segment)
                item.AcceptVisitor(visitor);

            Assert.Equal(refused, visitor.RangeContainsTextLocked());
        }

        // "La proposition du Conseil" + locked ", ainsi que": same text, the tail locked instead of tagged.
        private static ISegment SegmentWithLockedTail()
        {
            var items = new DocumentItemFactory();
            var properties = new PropertiesFactory();

            var locked = items.CreateLockedContent(properties.CreateLockedContentProperties(LockTypeFlags.Manual));
            locked.Content.Add(items.CreateText(properties.CreateTextProperties(After)));

            var segment = items.CreateSegment(items.CreateSegmentPairProperties());
            segment.Add(items.CreateText(properties.CreateTextProperties(Before)));
            segment.Add(locked);
            return segment;
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
